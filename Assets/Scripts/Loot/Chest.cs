using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] ChestRarity rarity;
    [SerializeField] List<ChestItem> mandatoryItems;

    Vector2 goldIntervalCommonChest = new(1, 100);
    Vector2 goldIntervalIncommonChest = new(101, 1000);
    Vector2 goldIntervalRareChest = new(1001, 5000);
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

    public float ReturnAmountOfGold() {
        float gold = 0f;
        switch (rarity) {
            case ChestRarity.Common:
                gold = Random.Range(goldIntervalCommonChest.x, goldIntervalCommonChest.y);
                break;
            case ChestRarity.Incommon:
                gold = Random.Range(goldIntervalIncommonChest.x, goldIntervalIncommonChest.y);
                break;
            case ChestRarity.Rare:
                gold = Random.Range(goldIntervalRareChest.x, goldIntervalRareChest.y);
                break;
        }

        return gold;
    }
}
