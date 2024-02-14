using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FOVVisualizer : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.viewPoint.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);
        Handles.DrawLine(fov.viewPoint.position, fov.viewPoint.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.viewPoint.position, fov.viewPoint.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;
        Handles.DrawLine(fov.viewPoint.position, fov.target.position);
    }
}