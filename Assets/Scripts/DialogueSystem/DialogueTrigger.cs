using Unity.Netcode;
using UnityEngine;

public abstract class DialogueTrigger : NetworkBehaviour
{
    public DialogueSO Dialogue;

    public void TriggerDialogue() {
        if (!IsServer) return;

        int dialogueId = DialogueManager.Instance.DialogueToInt(this);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }

    protected abstract void TryTrigger();
}
