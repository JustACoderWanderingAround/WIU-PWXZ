using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDoor : MonoBehaviour
{
    ModifyTransform modTransform;
    // Start is called before the first frame update
    void Start()
    {
        modTransform = GetComponent<ModifyTransform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            modTransform.Activate();
        }
    }
    private void OnTriggerExit(Collider other)
    {
       if (other.CompareTag("Enemy"))
        {
            modTransform.Reset();
        }
    }
}
