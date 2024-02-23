using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBuoyancy : MonoBehaviour
{
    [SerializeField]
    float subOffset = 0.5f;
    [SerializeField]
    float subRange = 1.0f;
    [SerializeField]
    float buoyancy = 1.5f;
    [SerializeField]
    float waterDrag = 1f;
    [SerializeField]
    Vector3 buoyancyOffset = Vector3.zero;

    Vector3 gravity;

    Rigidbody rb;
    float submergence;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        if (submergence > 0f)
        {
            float drag = Mathf.Max(0f, 1f - waterDrag * submergence * Time.deltaTime);
            rb.velocity *= drag;
            rb.angularVelocity *= drag;
            rb.AddForceAtPosition(
                gravity * -(buoyancy * submergence),
                transform.TransformPoint(buoyancyOffset),
                ForceMode.Acceleration
            );
            submergence = 0f;
        }
    }

    void CheckSubmergence()
    {
        if (Physics.Raycast(transform.position + transform.up * subOffset,
          -transform.up, out RaycastHit hit))
        {
            if (hit.collider.tag == "Water")
            {
                submergence = 1.0f - hit.distance / subRange;
            }
            else
            {
                submergence = 1f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            CheckSubmergence();
            rb.drag = waterDrag;
            rb.angularDrag = waterDrag;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!rb.IsSleeping() && other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            CheckSubmergence();
        }
    }
}
