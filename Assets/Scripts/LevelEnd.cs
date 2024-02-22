using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    [SerializeField] private Vector3 nextSpawnPos;
    [SerializeField] private bool isGameEnd;

    private void OnCollisionEnter(Collision col)
    {
        if (!isGameEnd)
            PlayerController.Instance.StartCoroutine(PlayerController.Instance.LoadLevel(nextLevel, nextSpawnPos));
        else
        {
            SceneManagement.Instance.LoadScene(nextLevel);
            SceneManagement.Instance.OnSceneLoaded(() => { Object.Destroy(PlayerController.Instance.transform.parent.gameObject); Time.timeScale = 1f; });
        }
    }
}