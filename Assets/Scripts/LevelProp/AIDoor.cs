using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDoor : MonoBehaviour
{
    GameObject activator;
    [SerializeField] ModifyTransform modTransform;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!modTransform.open && activator == null)
            {
                modTransform.AIActivate();
                activator = other.gameObject;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!modTransform.open && ((other == activator) && activator != null))
            {
                modTransform.Reset();
                activator = null;
            }
        }
    }
}
