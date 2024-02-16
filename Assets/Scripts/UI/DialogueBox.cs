using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] float textSpeed;
    [SerializeField] TMP_Text mainText;
    public bool finished;
    int currentLine;
    List<string> dialogueLines;
    public void InitDialogue(List<string> dialogues)
    {
        finished = false;
        mainText.text = string.Empty;
        dialogueLines = dialogues;
        currentLine = 0;
        StartDialogue();
    }
    private void Update()
    {
        if (dialogueLines != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                
            }
        }
    }
    public void StartDialogue()
    { 
        StartCoroutine(TypeLines());
    }
    IEnumerator TypeLines()
    {
        foreach (char character in dialogueLines[currentLine].ToCharArray())
        {
            mainText.text += character;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    void NextLine()
    {
        if (currentLine < dialogueLines.Count - 1)
        {
            currentLine++;
            mainText.text = string.Empty;
            StartDialogue();
        }
    }
    public void SkipThrough()
    {
        if (dialogueLines != null)
        {
            if (currentLine < dialogueLines.Count)
            {
                if (mainText.text == dialogueLines[currentLine])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    mainText.text = dialogueLines[currentLine];
                }
                if (currentLine == dialogueLines.Count - 1)
                {
                    finished = true;
                }
            }
        }
    }
}
