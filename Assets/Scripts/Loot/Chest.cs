using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : NetworkBehaviour {

    [Header("Chest Atributes")]
    [SerializeField] ChestRarity rarity;

    [Header("Loot")]
    [SerializeField, Tooltip("´Somente necessário se o baú for raro")] LegendaryRelic mandatoryMaevisLegendaryRelic;
    [SerializeField, Tooltip("´Somente necessário se o baú for raro")] LegendaryRelic mandatoryMelLegendaryRelic;
    [SerializeField, Tooltip("´Somente necessário se o baú for raro")] float mandatoryGold;
    int amountOfKeys;
    LegendaryRelic mandatoryLegendaryRelic;


    Vector2 goldIntervalCommonChest = new(1, 100);
    Vector2 goldIntervalIncommonChest = new(101, 1000);

    NetworkVariable<int> amountOfGold = new(0);
    CommonRelic fragment;
    List<PlayerController> players = new();

    public static event EventHandler MediumChestOpened;
    public static event EventHandler<ChestEventArgs> StatCommonChestCooldown;

    public class ChestEventArgs : EventArgs {
        public Chest chest;

        public ChestEventArgs(Chest chest) {
            this.chest = chest;
        }
    }

    public enum ChestRarity {
        Common,
        Medium,
        Rare
    }

    #region Initialize
    private void Start() {
        if (IsServer) {
            amountOfGold.Value = RandomizeGold();
        }
    }
    private void OnEnable() {
        if (IsServer) {
            amountOfGold.Value = RandomizeGold();
        }
    }
    int RandomizeGold() {
        float gold = 0f;
        switch (rarity) {
            case ChestRarity.Common:
                gold = Random.Range(goldIntervalCommonChest.x, goldIntervalCommonChest.y);
                break;
            case ChestRarity.Medium:
                gold = Random.Range(goldIntervalIncommonChest.x, goldIntervalIncommonChest.y);
                break;
            case ChestRarity.Rare:
                gold = mandatoryGold;
                break;
        }

        return (int)gold;
    }

    #endregion

    #region OpenChest
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame += Controller_OnInteract;
            controller.CanInteract = true;
            players.Add(controller);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame -= Controller_OnInteract;
            controller.CanInteract = false;
            players.Remove(controller);
        }
    }

    private void Controller_OnInteract(object sender, System.EventArgs e) {
        OpenChest();
    }
    void OpenChest() {
        AddFragmentToInventory();

        AddGoldToInventory();

        AddKeyToInventory();

        AddMandatoryItensToInventory();

        InvokeEvents();

        CloseChest();
    }

    private void InvokeEvents() {
        if (rarity == ChestRarity.Common) StatCommonChestCooldown?.Invoke(this, new ChestEventArgs(this));
        else if (rarity == ChestRarity.Medium) MediumChestOpened?.Invoke(this, EventArgs.Empty);

        ItenManager.Instance.TurnScreenOn(fragment, amountOfGold.Value, amountOfKeys, mandatoryLegendaryRelic);
    }

    void CloseChest() {
        var playersToRemove = new List<PlayerController>(players);

        foreach (var player in playersToRemove) {
            player.OnInteractInGame -= Controller_OnInteract;
            player.CanInteract = false;
        }

        players.Clear();

        gameObject.SetActive(false);
    }
    #endregion

    #region AddingToInventory
    void AddFragmentToInventory() {
        fragment = null;

        int rng = Random.Range(0, 100);

        switch (rarity) {
            case ChestRarity.Common:
                if (rng >= 75) ChooseFragment(); break;
            case ChestRarity.Medium:
                if (rng >= 25) ChooseFragment(); break;
            case ChestRarity.Rare:
                break;
        }
    }
    void ChooseFragment() {
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
    void AddGoldToInventory() {
        LocalWhiteBoard.Instance.AddGold(amountOfGold.Value);
    }
    void AddMandatoryItensToInventory() {
        if (mandatoryMaevisLegendaryRelic == null && mandatoryMelLegendaryRelic == null) return;

        mandatoryLegendaryRelic = null;

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) mandatoryLegendaryRelic = mandatoryMaevisLegendaryRelic;
        else mandatoryLegendaryRelic = mandatoryMelLegendaryRelic;

        LocalWhiteBoard.Instance.AddToLegendaryDictionary(mandatoryLegendaryRelic);
    }
    void AddKeyToInventory() {
        if (rarity != ChestRarity.Medium) return;

        if (LocalWhiteBoard.Instance.ReturnAmountOfKeys() == 3 || LocalWhiteBoard.Instance.ReturnFinalDoorOpened()) return;

        int rng = Random.Range(0, 100);

        if (rng <= (ChestManager.Instance.ReturnAmountOfMediumChestOpened() + 2) * 10) { amountOfKeys = 1; }
        else amountOfKeys = 0;

        LocalWhiteBoard.Instance.AddKey(amountOfKeys);
    }
    #endregion

}
