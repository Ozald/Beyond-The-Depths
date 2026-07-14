using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogueLine
{
    public string dialogue;
    public float delayBetweenCharacters; 
}

// Todo: Attach this to something with UI or a speech bubble
public class DialogueComponent : MonoBehaviour
{
    public List<DialogueLine> dialogueLines;
    public Buffer<DialogueLine> Lines = new Buffer<DialogueLine>();

    public void Start()
    {
        foreach (DialogueLine line in dialogueLines)
            Lines.Add(line);
    }

    /// <summary>
    /// Gets the next dialogue line
    /// </summary>
    /// <returns>
    /// The next dialogue line
    /// </returns>
    public DialogueLine NextLine()
    {
        return Lines.Remove();
    }
    
    /// <summary>
    /// Displays out a line of dialogue character by character
    /// </summary>
    /// <param name="line">
    /// The line of dialogue to show
    /// </param>
    public IEnumerator ShowDialogue(DialogueLine line)
    {
        string current = "";

        foreach (char c in line.dialogue)
        {
            if(c != '\n' && c != '\t' && c != ' ') // Shouldn't have a delay for blank characters
                yield return new WaitForSeconds(line.delayBetweenCharacters);
            
            current += c;
            Debug.Log(current);
        }
    }
}