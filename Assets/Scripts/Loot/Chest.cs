using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] ChestRarity rarity;
    public enum ChestRarity {
        Common,
        Incommon,
        Rare
    }

    void OpenChest() {
        ItenManager.Instance.TurnScreenOn();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;
        other.TryGetComponent<PlayerController>(out PlayerController controller);
        controller.OnInteractInGame += Controller_OnInteract;
        controller.CanInteract = true;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;
        other.TryGetComponent<PlayerController>(out PlayerController controller);
        controller.OnInteractInGame -= Controller_OnInteract;
        controller.CanInteract = false;
    }

    private void Controller_OnInteract(object sender, System.EventArgs e) {
        OpenChest();
    }
}
