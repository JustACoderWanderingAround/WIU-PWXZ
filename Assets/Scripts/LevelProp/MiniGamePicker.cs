using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class randomises and picks a minigame based on the attached list of minigames.
/// </summary>
public class MiniGamePicker : MonoBehaviour
{
    [SerializeField] GameObject miniGameContainer;
    [SerializeField] GameObject alertBoxCanvas;
    int chosenGame;
    bool withinRange, chosen;

    private void OnEnable()
    {
        withinRange = false;
        chosen = false;
    }
    void Activate()
    {
        //chosen = true;
        chosenGame = Random.Range(0, miniGameContainer.transform.childCount);
        miniGameContainer.transform.GetChild(chosenGame).gameObject.SetActive(true);
    }
    void Update()
    {
        if (withinRange)
        {
            if (!alertBoxCanvas.activeInHierarchy && !chosen)
                alertBoxCanvas.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Activate();
                chosen = true;
            }
        }
        else
        {
            if (alertBoxCanvas.activeInHierarchy || chosen)
                alertBoxCanvas.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        withinRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        withinRange = false;
    }
}
