using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public void InitNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public bool OnReachTarget(Transform target)
    {
        // Distance check to be changed to the thing wayne sent in discord
        if (Vector3.Distance(target.position, transform.position) <= 0.1f)
            return true;

        return false;
    }

    public void SetNavMeshTarget(Transform target, float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = target.position;
    }
}