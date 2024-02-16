using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject activeInventory;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text speechContent;

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = currentStamina;
        staminaBar.maxValue = maxStamina;
    }
    public void SetDialogueBoxActive(bool active)
    {
        dialogueBox.SetActive(active);
        activeInventory.SetActive(!active);
    }
    public void GetConversation(ConversationPartner convoPartner)
    {
        speakerNameText.text = convoPartner.GetData().speakerName;
        speechContent.text = convoPartner.GetData().dialogues[0];
    }
}