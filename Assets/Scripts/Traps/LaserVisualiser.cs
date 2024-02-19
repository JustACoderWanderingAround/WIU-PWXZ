using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserVisualiser : MonoBehaviour
{
    [Range(0f, float.MaxValue)]
    public float maxDistance = 1000f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (transform.rotation * Vector3.up) * maxDistance + transform.position);
    }
}
