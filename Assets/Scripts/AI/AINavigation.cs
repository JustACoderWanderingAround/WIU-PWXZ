using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private NavMeshAgent navMeshAgent;
    private int waypointIndex;

    public void InitNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public bool OnReachWaypoint()
    {
        // Distance check to be changed to the thing wayne sent in discord
        if (Vector3.Distance(waypoints[waypointIndex].position, transform.position) <= 0.1f)
            return true;

        return false;
    }

    public void UpdateNavMeshAgent(float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = waypoints[waypointIndex].position;

        if (!OnReachWaypoint())
            return;

        waypointIndex++;
        if (waypointIndex > waypoints.Length - 1)
            waypointIndex = 0;
    }
}