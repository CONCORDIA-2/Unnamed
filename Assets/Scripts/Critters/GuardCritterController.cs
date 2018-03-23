
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Networking;


public class GuardCritterController : NetworkBehaviour
{
    public NavMeshAgent agent;
    public Transform guardLocation;
    public float wanderRadius, wanderTimer, timer = 0;

    private void Awake()
    {
        wanderTimer = Random.Range(1, 3);
        agent = GetComponent<NavMeshAgent>();
        wanderRadius = guardLocation.transform.lossyScale.x / 2.0f;
    }

    private void Update()
    {

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            agent.destination = WanderLocation(guardLocation.position, wanderRadius, -1);
            timer = 0;
            wanderTimer = Random.Range(2, 6);
        }

    }

    private Vector3 WanderLocation(Vector3 origin, float dist, int layermask)
    {
        Vector3 newDirection = Random.insideUnitSphere * dist;
        newDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(newDirection, out navHit, dist, layermask);
        //Ensures that the new goal is on the navmesh, moves it onto the mesh close to the original goal if needed (found: https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/)

        return navHit.position;
    }

}
