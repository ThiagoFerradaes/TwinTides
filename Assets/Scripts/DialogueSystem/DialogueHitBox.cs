using Unity.Netcode;
using UnityEngine;

public class DialogueHitBox : NetworkBehaviour
{
    public DialogueSO Dialogue;


    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        int dialogueId = DialogueManager.Instance.DialogueToInt(this);

        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }
}
