using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    [SerializeField] float healPerSecond;
    [SerializeField] float cooldownBetweenHeals;
    Coroutine healCoroutine;

    HashSet<HealthManager> listOfPlayers = new();
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame += Controller_OnInteractInGame;
            controller.CanInteract = true;
        }

        CheckPointsManager.Instance.RegisterLastTotem(this);

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        listOfPlayers.Add(health);

        health.CleanAllDebuffs();

        healCoroutine ??= StartCoroutine(HealRoutine());
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame -= Controller_OnInteractInGame;
            controller.CanInteract = false;
        }

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        listOfPlayers.Remove(health);

        if (listOfPlayers.Count == 0) {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    private void Controller_OnInteractInGame(object sender, System.EventArgs e) {
        if (sender is MonoBehaviour mono) TotemManager.Instance.TurnTotemScreenOn(mono.gameObject);
    }

    IEnumerator HealRoutine() {
        while (true) {
            foreach (var player in listOfPlayers) player.Heal(healPerSecond, false);
            yield return new WaitForSeconds(cooldownBetweenHeals);
        }
    }
}
