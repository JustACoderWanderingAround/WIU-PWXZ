using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private GameObject activeInventory;
    [SerializeField] private GameObject playerDialogueBox;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text speechContent;

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = currentStamina;
        staminaBar.maxValue = maxStamina;
        if (currentStamina >= maxStamina)
            staminaBar.gameObject.SetActive(false);
        else
            staminaBar.gameObject.SetActive(true);
        if (dialogueBox?.finished  ?? false)
            SetDialogueBoxActive(false);
    }
    public void SetDialogueBoxActive(bool active)
    {
        playerDialogueBox?.SetActive(active);
        activeInventory.SetActive(!active);
    }
    public void GetConversation(ConversationPartner convoPartner)
    {
        speakerNameText.text = convoPartner.GetData().speakerName;
        dialogueBox.InitDialogue(convoPartner.GetData().dialogues);
    }
    public void SkipThruText()
    {
        dialogueBox.SkipThrough();

       
    }
}