using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    [SerializeField] private Vector3 nextSpawnPos;

    private void OnCollisionEnter(Collision col)
    {
        StartCoroutine(PlayerController.Instance.LoadLevel(nextLevel, nextSpawnPos));
    }
}
