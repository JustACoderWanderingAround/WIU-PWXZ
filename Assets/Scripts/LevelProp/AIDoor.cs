using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDoor : MonoBehaviour
{
    [SerializeField] ModifyTransform modTransform;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("open");
            if (!modTransform.open)
                modTransform.AIActivate();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!modTransform.open)
                modTransform.Reset();
        }
    }
}
