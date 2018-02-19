
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Networking;

public class CritterController : NetworkBehaviour
{
    private CritterController instance;
    public static bool playerIsDown = false; //True after a successful attack, must set false again after players revive one another
    public static bool separatedTooLong;

    public NavMeshAgent agent;
    public GameObject player1, player2, guardLocation;

    public TreeNode root;

    public float acceptableDistanceToGoal = 1;
    public float attackDistance = 2;
    public bool holdingHands;

    public bool foundPlayers = false;

    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    [SerializeField] private GameObject localPlayerManager;

    void Awake()
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
            NodeStatus treeStatus = root.Process();
        } else if (foundPlayers)
        {
            NodeStatus treeStatus = root.Process();
        } else
        {
            Spawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Need to tell player it is dead and play attack animation
            playerIsDown = true;
            Debug.Log("Hit " + collision.gameObject.tag);
            CritterSpawner.critterCount--;
            UnityEngine.Object.Destroy(instance.agent.gameObject);
        }
    }

    private void Spawn()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();

        getPlayers();

        guardLocation = GameObject.FindGameObjectWithTag("Guard");
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
        sl1.children[0] = sq1;
        sl1.children[1] = sq3;
        sl1.children[2] = new Retreat(FindNearestGoal(instance), instance);

        sq3.children[0] = new WalkTo(instance.guardLocation, instance);   //Guarding a location
        sq3.children[1] = new IdleOnScreen();

        sq1.children[0] = sq2;
        sq1.children[1] = new Retreat(FindNearestGoal(instance), instance);

        sq2.children[0] = sl2;
        sq2.children[1] = new WalkTo((instance.FindClosestPlayer()), instance); //attacking the player

        sl2.children[0] = new ProximityAttack(instance);
        sl2.children[1] = new TimedAttack(instance);
        

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

    public WalkTo(GameObject goal, CritterController instance)
    {
        this.goal = goal;
        this.instance = instance;
        Debug.Log("Walking to: " + this.goal.name);
    }

    public override NodeStatus Process()
    {
        instance.agent.destination = goal.transform.position;    //Move to the potentially moving goal (player, guard station, or exit)
        if (instance.agent.remainingDistance < acceptableDistanceToGoal)
            return NodeStatus.SUCCESS;
        else
            return NodeStatus.RUNNING;
    }

}

public class Retreat : Decorator
{
    GameObject goal;

    public Retreat(GameObject goal, CritterController instance)
    {
        this.goal = goal;
        this.instance = instance;
        Debug.Log("Retreating");
        child = new WalkTo(goal, instance);
    }

    public override NodeStatus Process()
    {
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
        Debug.Log("Idling");
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
        if (instance.FindDistanceToPlayer(instance.FindClosestPlayer()) <= instance.attackDistance && !instance.holdingHands && !CritterController.playerIsDown)
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
        if (CritterController.separatedTooLong && !CritterController.playerIsDown)
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