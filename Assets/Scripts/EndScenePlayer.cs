using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndScenePlayer : MonoBehaviour
{
    [SerializeField] private GameObject defaultDirector;
    [SerializeField] private GameObject whistleDirector;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (GameManager.GetEvidenceList().Count > 2)
        {
            whistleDirector.SetActive(true);
        }
        else
        {
            defaultDirector.SetActive(true);
        }
    }
}
