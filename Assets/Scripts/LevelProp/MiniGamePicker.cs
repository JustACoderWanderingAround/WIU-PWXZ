using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class randomises and picks a minigame based on the attached list of minigames.
/// </summary>
public class MiniGamePicker : MonoBehaviour, IInteractable
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
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))  
            if (!alertBoxCanvas.activeInHierarchy && !chosen)
                alertBoxCanvas.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            if (alertBoxCanvas.activeInHierarchy || chosen)
                alertBoxCanvas.SetActive(false);
    }
    public void Reset()
    {
        chosen = false;
    }

    public void OnInteract()
    {
        Activate();
        chosen = true;
    }
}
