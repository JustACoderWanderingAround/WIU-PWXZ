using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public static RigController Instance;

    [SerializeField] private Transform leftHandGrabTarget;
    [SerializeField] private Transform rightHandGrabTarget;

    private void Awake()
    {
        Instance = this;
    }

    public void LeftHandGrab(Transform target)
    {
        leftHandGrabTarget.position = target.position;
        leftHandGrabTarget.position = target.position;
    }

    public void RightHandGrab(Transform target)
    {
        rightHandGrabTarget.position = target.position;
        rightHandGrabTarget.position = target.position;
    }
}