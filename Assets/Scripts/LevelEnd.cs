using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private string nextLevel;

    private void OnCollisionEnter(Collision col)
    {
        SceneManagement.Instance.LoadScene(nextLevel);
    }
}
