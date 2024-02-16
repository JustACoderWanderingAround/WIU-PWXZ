using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string speakerName;
    public List<string> dialogues = new List<string>();
    List<string> GetDialogue()
    {
        return dialogues;
    }
}
