using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IChestItem {
    public void AddItemToInventory() { }
}
public class Chest : NetworkBehaviour {
    [SerializeField] ChestRarity rarity;
    [SerializeField] List<ChestItem> mandatoryItems;

    Vector2 goldIntervalCommonChest = new(1, 100);
    Vector2 goldIntervalIncommonChest = new(101, 1000);
    Vector2 goldIntervalRareChest = new(1001, 5000);

    NetworkVariable<int> amountOfGold = new(0);
    public CommonRelic fragment;

    public enum ChestRarity {
        Common,
        Incommon,
        Rare
    }

    private void Start() {
        if (IsServer) {
            amountOfGold.Value = RandomizeGold();
        }
    }

    void OpenChest() {
        int rng = Random.Range(0, 100);

        switch (rarity) {
            case ChestRarity.Common:
                if (rng >= 50) RandomizeFragment(); break;
            case ChestRarity.Incommon:
                if (rng >= 25) RandomizeFragment(); break;
            case ChestRarity.Rare:
                RandomizeFragment(); break;
        }

        ItenManager.Instance.TurnScreenOn(fragment, amountOfGold.Value);
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

    int RandomizeGold() {
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

        return (int)gold;
    }

    void RandomizeFragment() {
        if (LocalWhiteBoard.Instance.CheckIfAllRelicsAreMaxed()) { fragment = null; return; }

        var commonSkills = PlayerSkillConverter.Instance.ReturnCommonSkillList(LocalWhiteBoard.Instance.PlayerCharacter);
        int skillCounter = 0;
        int rng = Random.Range(0, commonSkills.Count);

        while (skillCounter < commonSkills.Count) {
            CommonRelic relic = commonSkills[rng] as CommonRelic;

            bool alreadyHaveRelic = LocalWhiteBoard.Instance.CheckIfCommonRelicAlredyExist(relic);
            bool fragmentsMaxed = LocalWhiteBoard.Instance.CheckIfAlredyHaveMaxFragments(relic);

            if (!alreadyHaveRelic) {
                LocalWhiteBoard.Instance.AddToCommonDictionary(relic);
                fragment = relic;
                break;
            }
            else if (!fragmentsMaxed) {
                LocalWhiteBoard.Instance.AddFragment(relic);
                fragment = relic;
                break;
            }
            else {
                rng++;
                if (rng >= commonSkills.Count) rng = 0;
                skillCounter++;
            }
        }
    }


    public float ReturnGoldAmount() {
        return amountOfGold.Value;
    }
}
