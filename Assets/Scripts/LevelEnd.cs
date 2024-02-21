using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    [SerializeField] private Vector3 nextSpawnPos;

    private void OnCollisionEnter(Collision col)
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        SceneManagement.Instance.LoadScene(nextLevel);

        while (SceneManagement.Instance.isLoading)
        {
            yield return null;
        }

        PlayerController.Instance.gameObject.transform.position = nextSpawnPos;
    }
}
