using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float viewAngle;
    public float viewRadius;
    public Transform viewPoint;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [HideInInspector]
    public Transform target;

    public Vector3 DirFromAngle(float angle, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
            angle += viewPoint.eulerAngles.y;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool FindVisibleTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(viewPoint.position, viewRadius, targetMask);
        foreach(Collider col in colliders)
        {
            Vector3 dir = (col.transform.position - viewPoint.position).normalized;

            if (Vector3.Angle(viewPoint.forward, dir) < viewAngle / 2)
            {
                float dist = Vector3.Distance(viewPoint.position, col.transform.position);

                if (!Physics.Raycast(viewPoint.position, dir, dist, obstacleMask))
                {
                    target = col.transform;
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckTargetInLineOfSight(out Vector3 lastSeenPosition, float distance)
    {
        Vector3 dir = (target.position - viewPoint.position).normalized;
        lastSeenPosition = target.position;

        if (!Physics.Raycast(viewPoint.position, dir, distance, obstacleMask))
            return true;

        target = null;
        return false;
    }
}