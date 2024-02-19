using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConversationPartner : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    public DialogueData GetData()
    {
        return dialogueData;
    }
}
