using UnityEngine;

public class Totem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame += Controller_OnInteractInGame;
            controller.CanInteract = true;
        }

        CheckPointsManager.Instance.RegisterLastTotem(this);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame -= Controller_OnInteractInGame;
            controller.CanInteract = false;
        }
    }

    private void Controller_OnInteractInGame(object sender, System.EventArgs e) {
        TotemManager.Instance.TurnTotemScreenOn();
    }
}
