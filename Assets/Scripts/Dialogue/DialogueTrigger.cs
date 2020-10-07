using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, ISaveable
{
    public Dialogue dialogue;
    private bool dialogueAlreadyTriggered;    

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !dialogueAlreadyTriggered)
        {
            dialogueAlreadyTriggered = true;
            GetComponent<CircleCollider2D>().enabled = false;
            TriggerDialogue();
        }
    }

    public object CaptureState()
    {
        return dialogueAlreadyTriggered;
    }

    public void RestoreState(object state)
    {
        dialogueAlreadyTriggered = (bool)state;
    }
}
