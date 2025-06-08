using Unity.Netcode;
using UnityEngine;

public class DialogueHitBox : DialogueTrigger {
    protected override void TryTrigger() {}

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        TriggerDialogue();
    }
}
