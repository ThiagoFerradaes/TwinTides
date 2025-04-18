using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class LocalWhiteBoard : MonoBehaviour {
    public static LocalWhiteBoard Instance;


    public Characters PlayerCharacter;
    public bool IsSinglePlayer;
    public bool AnimationOn;
    bool finalDoorOpened;

    public CommonRelic PlayerCommonRelicSkillOne;
    public CommonRelic PlayerCommonRelicSkillTwo;
    public LegendaryRelic PlayerLegendarySkill;
    public AttackSkill PlayerAttackSkill;

    public Dictionary<CommonRelic, int> CommonRelicInventory = new();
    public Dictionary<LegendaryRelic, int> LegendaryRelicInventory = new();
    public Dictionary<CommonRelic, int> FragmentsInventory = new();
    public int AttackLevel;
    int AmountOsKeys;

    public float Gold;

    [HideInInspector] public float GeneralVolume = 0.5f;
    [HideInInspector] public float MusicVolume = 0.5f;
    [HideInInspector] public float SFXVolume = 0.5f;
    [HideInInspector] public float AmbienceVolume = 0.5f;
    [HideInInspector] public float DialogueVolume = 0.5f;

    public static event EventHandler OnRelicEquiped;
    public static event EventHandler OnGoldChanged;


    private void Awake() {  // Singleton, só vai ter um LocalWhiteBoard no jogo
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Adicionando a relíquia ao dicionário de relíquias
    // Só é pra ser chamado 1 vez, na primeira vez que o jogador recebe a relíquia
    public void AddToCommonDictionary(CommonRelic relic) {
        if (CommonRelicInventory.ContainsKey(relic) || relic.Character != PlayerCharacter) return;

        CommonRelicInventory.Add(relic, 1);
        FragmentsInventory.Add(relic, 0);
    }

    // Adicionando a relíquia ao dicionário de relíquias
    // Só é pra ser chamado 1 vez, na primeira vez que o jogador recebe a relíquia
    public void AddToLegendaryDictionary(LegendaryRelic relic) {
        if (LegendaryRelicInventory.ContainsKey(relic) || relic.Character != PlayerCharacter) return;

        LegendaryRelicInventory.Add(relic, 1);
    }

    public void UpdateCommonRelicLevel(CommonRelic relic, int level) {
        if (!CommonRelicInventory.ContainsKey(relic)) return;

        CommonRelicInventory[relic] = Mathf.Clamp(level, 1, relic.MaxLevel);
    }

    /// <summary>
    /// index 1 = slot 1, index 2 == slot 3 else == legendary
    /// </summary>
    /// <param name="relic"></param>
    /// <param name="index"></param>
    public void EquipRelic(Skill relic, int index) {
        if (relic.Character != PlayerCharacter) return;

        if (relic is CommonRelic && !CommonRelicInventory.ContainsKey(relic as CommonRelic)) return;

        if (relic is LegendaryRelic && !LegendaryRelicInventory.ContainsKey(relic as LegendaryRelic)) return;

        if (index == 1) {
            if (relic == PlayerCommonRelicSkillTwo) {
                if (PlayerCommonRelicSkillOne == null) PlayerCommonRelicSkillTwo = null;
                else PlayerCommonRelicSkillTwo = PlayerCommonRelicSkillOne;
            }
            PlayerCommonRelicSkillOne = relic as CommonRelic;
        }
        else if (index == 2) {
            if (relic == PlayerCommonRelicSkillOne) {
                if (PlayerCommonRelicSkillTwo == null) PlayerCommonRelicSkillOne = null;
                else PlayerCommonRelicSkillOne = PlayerCommonRelicSkillTwo;
            }
            PlayerCommonRelicSkillTwo = relic as CommonRelic;
        }
        else { PlayerLegendarySkill = relic as LegendaryRelic; }

        OnRelicEquiped?.Invoke(this, EventArgs.Empty);
    }

    public Skill ReturnCurrentSkill(int skillIndex) {
        return skillIndex switch {
            1 => PlayerCommonRelicSkillOne,
            2 => PlayerCommonRelicSkillTwo,
            _ => PlayerLegendarySkill,
        };
    }

    public int ReturnCurrentSkillLevel(int skillIndex) {
        return skillIndex switch {
            1 => CommonRelicInventory[PlayerCommonRelicSkillOne],
            2 => CommonRelicInventory[PlayerCommonRelicSkillTwo],
            _ => LegendaryRelicInventory[PlayerLegendarySkill],
        };
    }

    /// <summary>
    /// Verifica se o jogador já possui a relíquia registrada
    /// </summary>
    /// <param name="relic"></param>
    /// <returns></returns>
    public bool CheckIfCommonRelicAlredyExist(CommonRelic relic) {
        if (CommonRelicInventory.ContainsKey(relic) && FragmentsInventory.ContainsKey(relic)) return true;
        return false;
    }

    /// <summary>
    /// Retorna se o jogador já possui o máximo de fragmentos ou se o nível da relíquia já está no nível 4
    /// </summary>
    /// <param name="relic"></param>
    /// <returns></returns>
    public bool CheckIfAlredyHaveMaxFragments(CommonRelic relic) {
        if (!CommonRelicInventory.ContainsKey(relic) || !FragmentsInventory.ContainsKey(relic)) return false; 

        return (CommonRelicInventory[relic] + FragmentsInventory[relic]) >= 4;
    }

    /// <summary>
    /// Verifica se todas as relíquias comuns já estão maximizadas
    /// </summary>
    /// <returns></returns>
    public bool CheckIfAllRelicsAreMaxed() {
        if (CommonRelicInventory.Count != 5) return false;

        int skillCounter = 0;

        foreach (var skill in PlayerSkillConverter.Instance.ReturnCommonSkillList(PlayerCharacter)) {
            if (CheckIfAlredyHaveMaxFragments(skill as CommonRelic)) skillCounter++;
        }

        return skillCounter == 5;
    }

    /// <summary>
    /// Aumenta em 1 o número de framentos no inventário
    /// </summary>
    /// <param name="relic"></param>
    public void AddFragment(CommonRelic relic) {
        FragmentsInventory[relic]++;
    }

    public int ReturnFragmentsAmount(CommonRelic relic) {
        return FragmentsInventory[relic];
    }

    public void AddGold(float goldAmount) {
        Gold += goldAmount;
        OnGoldChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveGold(float goldAmount) {
        Gold -= goldAmount;
        OnGoldChanged?.Invoke(this, EventArgs.Empty);
    }

    public float ReturnGoldAmount() {
        return Gold;
    }

    public void AddKey(int keyAmount) {
        AmountOsKeys += keyAmount;
    }

    public void RemoveKey(int keyAmount) {
        AmountOsKeys -= keyAmount;
    }

    public int ReturnAmountOfKeys() {
        return AmountOsKeys;
    }

    public bool ReturnFinalDoorOpened() {
        return finalDoorOpened;
    }

    public int ReturnSkillLevel(Skill skill) {
        if (skill is CommonRelic) {
            return CommonRelicInventory[skill as CommonRelic];
        }
        else {
            return LegendaryRelicInventory[skill as LegendaryRelic];
        }
    }
}
