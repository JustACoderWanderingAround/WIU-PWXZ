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

    public bool OnReachTarget(Vector3 target)
    {
        // Distance check to be changed to the thing wayne sent in discord
        if (Vector3.Distance(target, transform.position) <= 0.3f)
            return true;

        return false;
    }

    public void SetNavMeshTarget(Vector3 target, float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = target;
    }
    public void StopNavigation()
    {
        navMeshAgent.speed = 0;
    }
}