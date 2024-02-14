using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float viewAngle;
    public float viewRadius;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    public List<Transform> targets = new List<Transform>();

    public Vector3 DirFromAngle(float angle, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
            angle += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private void FindVisibleTargets()
    {
        targets.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        foreach(Collider col in colliders)
        {
            Vector3 dir = (col.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dir) < viewAngle / 2)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);

                if (!Physics.Raycast(transform.position, dir, dist, obstacleMask))
                    targets.Add(col.transform);
            }
        }
    }

    private void Update()
    {
        FindVisibleTargets();
    }
}