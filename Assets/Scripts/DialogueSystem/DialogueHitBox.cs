using Unity.Netcode;
using UnityEngine;

public class DialogueHitBox : NetworkBehaviour {
    [SerializeField] DialogueSO dialogue;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        TriggerDialogue();

        gameObject.SetActive(false);
    }
    public void TriggerDialogue() {
        if (!IsServer) return;

        int dialogueId = DialogueManager.Instance.DialogueToInt(dialogue);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }
}
