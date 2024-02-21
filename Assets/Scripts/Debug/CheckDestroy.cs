using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log(gameObject + " has been Destroyed");
    }
}
