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

    public void UpdateNavMeshAgent()
    {
        // Distance check to be changed to the thing wayne sent in discord
        if (Vector3.Distance(waypoints[waypointIndex].position, transform.position) <= 0.1f)
        {
            waypointIndex++;
            if (waypointIndex > waypoints.Length - 1)
                waypointIndex = 0;
        }

        navMeshAgent.destination = waypoints[waypointIndex].position;
    }

    // Temporary for testing

    private void Start()
    {
        InitNavMeshAgent();
    }

    private void Update()
    {
        UpdateNavMeshAgent();
    }
}