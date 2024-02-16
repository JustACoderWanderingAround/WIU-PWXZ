using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    // dialogueBox;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text speechContent;

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = currentStamina;
        staminaBar.maxValue = maxStamina;
    }
}