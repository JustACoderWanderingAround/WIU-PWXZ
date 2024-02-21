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
    public Vector3 targetPos;

    public Vector3 DirFromAngle(float angle, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
            angle += viewPoint.eulerAngles.y;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool FindVisibleTargets(out List<Collider> targets)
    {
        Collider[] colliders = Physics.OverlapSphere(viewPoint.position, viewRadius, targetMask);
        List<Collider> targetCols = new List<Collider>();

        foreach(Collider col in colliders)
        {
            Vector3 dir = (col.transform.position - viewPoint.position).normalized;

            if (Vector3.Angle(viewPoint.forward, dir) < viewAngle / 2)
            {
                float dist = Vector3.Distance(viewPoint.position, col.transform.position);

                if (!Physics.Raycast(viewPoint.position, dir, dist, obstacleMask))
                    targetCols.Add(col);
            }
        }

        if (targetCols.Count > 0)
        {
            targets = targetCols;
            return true;
        }

        targets = new List<Collider>();
        return false;
    }

    public bool FindVisibleTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(viewPoint.position, viewRadius, targetMask);
        foreach (Collider col in colliders)
        {
            Vector3 dir = (col.transform.position - viewPoint.position).normalized;

            if (Vector3.Angle(viewPoint.forward, dir) < viewAngle / 2)
            {
                float dist = Vector3.Distance(viewPoint.position, col.transform.position);

                if (!Physics.Raycast(viewPoint.position, dir, dist, obstacleMask))
                {
                    targetPos = col.transform.position;
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckTargetInLineOfSight(out Vector3 lastSeenPosition, float distance)
    {
        Vector3 dir = (targetPos - viewPoint.position).normalized;
        lastSeenPosition = targetPos;

        if (!Physics.Raycast(viewPoint.position, dir, distance, obstacleMask))
            return true;

        targetPos = Vector3.zero;
        return false;
    }

    public bool CheckInteractableInteracted(out Vector3 targetPos, out IInteractable targetInteractable)
    {
        targetInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(viewPoint.position, viewRadius);
        foreach (Collider col in colliders)
        {
            if (!col.CompareTag("Interactable"))
                continue;

            Vector3 dir = (col.transform.position - viewPoint.position).normalized;

            if (Vector3.Angle(viewPoint.forward, dir) < viewAngle / 2)
            {
                float dist = Vector3.Distance(viewPoint.position, col.transform.position);
                RaycastHit hit;
                if (Physics.Raycast(viewPoint.position, dir, out hit, dist, obstacleMask))
                {
                    if (!hit.collider.gameObject.CompareTag("Interactable"))
                        break;

                    IInteractable interactable = col.gameObject.GetComponent<IInteractable>();

                    if (interactable == null)
                        continue;

                    if (interactable.isInteracted)
                    {
                        targetInteractable = interactable;
                        targetPos = col.transform.position + -col.transform.right * 2f;
                        return true;
                    }
                    else
                    {
                        targetPos = Vector3.zero;
                        return false;
                    }
                }
            }
        }

        targetPos = Vector3.zero;
        return false;
    }
}