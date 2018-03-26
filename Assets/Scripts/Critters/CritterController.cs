
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Networking;


public class CritterController : NetworkBehaviour
{
    private CritterController instance;
    public CritterSpawner mySpawner;
    public static bool playerIsDown = false; //True after a successful attack, must set false again after players revive one another
    public bool separatedTooLong = false;

    public NavMeshAgent agent;
    public GameObject player1, player2, guardLocation;

    public TreeNode root;

    public float player1Sanity, player2Sanity;
    public float acceptableDistanceToGoal = 1;
    private float attackDistance;

    public bool foundPlayers = false;

    public GameObject explosionPrefab;

    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    [SerializeField] private GameObject localPlayerManager;

    private void Awake()
    {
        // grab reference to the local player manager if not assigned
        localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        if (localPlayerManager)
            localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

        Spawn();
    }

    private void Update()
    {
        if (foundPlayers && root == null)
        {
            root = BuildTree(instance);
        }
        else
        {
            Spawn();
        }
        if (foundPlayers)
        {
            player1Sanity = player1.GetComponent<SanityAndLight>().sanityLevel;
            player2Sanity = localPlayerManagerScript.GetOtherSanityLevel();
            if (player1Sanity <= 5 || player2Sanity <= 5)
                separatedTooLong = true;
            else
                separatedTooLong = false;
        }
        root.Process();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Need to play attack animation
            playerIsDown = true;
            collision.gameObject.GetComponent<SanityAndLight>().isIncapacitated = playerIsDown;
            collision.gameObject.GetComponent<SanityAndLight>().CmdSetOtherIsIncapacitated(collision.gameObject.GetComponent<SanityAndLight>().playerControllerId, playerIsDown);
            lock (mySpawner.countLock)
            {
               if (mySpawner.critterCount >= 1)
                     mySpawner.critterCount--;
            }
            UnityEngine.Object.Destroy(instance.agent.gameObject);

            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(explosion);
            Destroy(explosion, 2.0f);
        }
    }

    protected void Spawn()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();

        getPlayers();

        agent.speed = Random.Range(3, 5);
    }

    private void getPlayers()
    {
        player1 = localPlayerManagerScript.GetLocalPlayerObject();
        player2 = localPlayerManagerScript.GetOtherPlayerObject();
        if (player1 && player2)
            foundPlayers = true;
    }

    public static TreeNode BuildTree(CritterController instance)
    {
        Repeater root = new Repeater(instance);
        Selector sl1 = new Selector(instance, 3), sl2 = new Selector(instance, 2);
        Sequence sq1 = new Sequence(instance, 2), sq2 = new Sequence(instance, 2), sq3 = new Sequence(instance, 2);


        root.child = sl1;
        sl1.children[0] = sl2;
        sl1.children[1] = sq3;
        sl1.children[2] = new Retreat(FindNearestGoal(instance), instance);

        sq3.children[0] = new WalkTo(instance.guardLocation, instance);   //Guarding a location
        sq3.children[1] = new IdleOnScreen();

        sq1.children[0] = new ProximityAttack(instance);
        sq1.children[1] = new WalkTo((instance.FindClosestPlayer()), instance); //attacking the closest player

        sq2.children[0] = new TimedAttack(instance);
        sq2.children[1] = new WalkTo((instance.FindInsanePlayer()), instance); //attacking the insane player

        sl2.children[0] = sq1;
        sl2.children[1] = sq2;

        return root;
    }

    //Helper methods:

    public static GameObject FindNearestGoal(CritterController instance)
    {
        return instance.guardLocation; //Until levels are blocked, ticks will despawn at their guard location instead of trying to locate the nearest exit
    }

    public float FindDistanceToPlayer(GameObject player)
    {
        float dis = Vector3.Distance(agent.transform.position, player.transform.position);
        return dis;
    }

    public GameObject FindClosestPlayer()
    {
        if (FindDistanceToPlayer(player1) < FindDistanceToPlayer(player2))
            return player1;
        else
            return player2;
    }

    public GameObject FindInsanePlayer()
    {
        if (player1Sanity <= player2Sanity)
            return player1;
        else
            return player2;
    }

    public float getAttackDistance()
    {
        return attackDistance;
    }

    public void setAttackDistance(float newD)
    {
        attackDistance = newD;
    }

    public Vector3 WanderLocation(Vector3 origin, float dist, int layermask)
    {
        Vector3 newDirection = Random.insideUnitSphere * dist;
        newDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(newDirection, out navHit, dist, layermask);
        //Ensures that the new goal is on the navmesh, moves it onto the mesh close to the original goal if needed (found: https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/)

        return navHit.position;
    }

    public void PlaySFX_Footsteps()
    {
        GetComponent<AudioSource>().Play();
    }
}

public enum NodeStatus { SUCCESS, FAILURE, RUNNING }

public abstract class TreeNode
{
    public CritterController instance;
    public abstract NodeStatus Process();

}


public abstract class Composite : TreeNode
{
    public TreeNode[] children;
}

public class Sequence : Composite   //Essentialy an AND gate for its child processes
{
    public Sequence(CritterController instance, int numOfChildren)
    {
        this.instance = instance;
        children = new TreeNode[numOfChildren];
    }

    public override NodeStatus Process()
    {
        foreach (TreeNode child in children)
        {
            NodeStatus childStatus = child.Process();
            if (childStatus != NodeStatus.SUCCESS)
                return childStatus;
        }
        return NodeStatus.SUCCESS;
    }
}

public class Selector : Composite   //Essentialy an OR gate for its child processes
{
    public Selector(CritterController instance, int numOfChildren)
    {
        this.instance = instance;
        children = new TreeNode[numOfChildren];
    }


    public override NodeStatus Process()
    {
        foreach (TreeNode child in children)
        {
            NodeStatus childStatus = child.Process();
            if (childStatus != NodeStatus.FAILURE)
                return childStatus;
        }
        return NodeStatus.FAILURE;
    }
}

public abstract class Decorator : TreeNode
{
    public TreeNode child;
}

public class Inverter : Decorator    //Essentialy a NOT gate for its child
{
    public Inverter(CritterController instance)
    {
        this.instance = instance;
    }
    public override NodeStatus Process()
    {
        if (child.Process() == NodeStatus.SUCCESS)
            return NodeStatus.FAILURE;
        else if (child.Process() == NodeStatus.FAILURE)
            return NodeStatus.SUCCESS;
        else
            return NodeStatus.RUNNING;
    }
}

public class Succeeder : Decorator  //Essentially a TRUE
{
    public override NodeStatus Process()
    {
        return NodeStatus.SUCCESS;
    }
}

public class Repeater : Decorator
{

    public Repeater(CritterController instance)
    {

        this.instance = instance;
    }

    public override NodeStatus Process()
    {

        return child.Process();
    }

}

public class RepeatUntilFail : Decorator
{
    bool failed = false;

    public RepeatUntilFail(CritterController instance)
    {
        this.instance = instance;
    }

    public override NodeStatus Process()
    {
        while (!failed)
        {
            if (child.Process() == NodeStatus.FAILURE)
                failed = true;
        }
        return NodeStatus.SUCCESS;
    }
}

public class WalkTo : TreeNode
{
    public GameObject goal;
    public float acceptableDistanceToGoal;

    //For chasing a player
    bool attackingPlayer = false;

    public WalkTo(GameObject goal, CritterController instance)
    {
        this.goal = goal;
        this.instance = instance;
        if (goal.tag == "Player")
            attackingPlayer = true;
    }

    public override NodeStatus Process()
    {
        //Debug.Log("Walking to: " + this.goal.name);
        instance.agent.destination = goal.transform.position;    //Move to the potentially moving goal (player, guard station, or exit)
        if (!attackingPlayer)
        {
            instance.agent.destination = instance.WanderLocation(goal.transform.position, 2.5f, -1);    //wander near the guard station instead of hugging it
        }
        bool inRadius = Vector3.Distance(instance.guardLocation.transform.position, goal.transform.position) < instance.getAttackDistance();

        if (instance.agent.remainingDistance < acceptableDistanceToGoal)   //Managed to hit a player (or reached the guard spot)
            return NodeStatus.SUCCESS;
        else if (attackingPlayer && inRadius) //A player is too close to the guard spot, insane or not
            return NodeStatus.RUNNING;
        else if (attackingPlayer && !instance.separatedTooLong) //players have returned to safety with one another
            return NodeStatus.FAILURE;
        else
            return NodeStatus.RUNNING;      //None of the above, still in pursuit
    }

}

public class Retreat : Decorator
{

    public Retreat(GameObject goal, CritterController instance)
    {
        this.instance = instance;
        child = new WalkTo(goal, instance);
    }

    public override NodeStatus Process()
    {
        Debug.Log("RETREATING");
        NodeStatus childStatus = child.Process();
        if (childStatus == NodeStatus.RUNNING)
            return NodeStatus.RUNNING;
        else if (child.Process() == NodeStatus.SUCCESS)
        {
            //Play escape animation (along a wall or into a pipe)
            UnityEngine.Object.Destroy(instance.agent.gameObject);
            return NodeStatus.SUCCESS;
        }
        else
            return NodeStatus.FAILURE;  //What to do if they fail to retreat? Blow up?
    }
}

public class IdleOnScreen : TreeNode
{
    public override NodeStatus Process()
    {
        //Debug.Log("Idling");
        //play wandering animation
        return NodeStatus.SUCCESS;
    }
}

public class ProximityAttack : TreeNode
{
    public ProximityAttack(CritterController instance)
    {
        this.instance = instance;
    }

    public override NodeStatus Process()
    {
        GameObject closePlayer = instance.FindClosestPlayer();
        bool inRadius = Vector3.Distance(instance.guardLocation.transform.position, closePlayer.transform.position) < instance.getAttackDistance();
        //Debug.Log("Closest player's distance to guardLocation = " + Vector3.Distance(instance.guardLocation.transform.position, closePlayer.transform.position));
        if (!CritterController.playerIsDown && inRadius)
        {
            //Debug.Log("Proximity attack initiated");
            return NodeStatus.SUCCESS;
        }
        else
        {
            return NodeStatus.FAILURE;
        }
    }
}

public class TimedAttack : TreeNode
{
    public TimedAttack(CritterController instance)
    {
        this.instance = instance;
    }

    public override NodeStatus Process()
    {
        if (instance.separatedTooLong && !CritterController.playerIsDown)
        {
            //Debug.Log("Timed attack available");
            return NodeStatus.SUCCESS;
        }
        else
        {
            return NodeStatus.FAILURE;
        }
    }
}