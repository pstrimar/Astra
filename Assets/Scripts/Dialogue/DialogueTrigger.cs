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
        // Start dialogue if it hasn't already happened
        if (collision.tag == "Player" && !dialogueAlreadyTriggered)
        {
            dialogueAlreadyTriggered = true;
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
