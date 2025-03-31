using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalWhiteBoard : MonoBehaviour {
    public static LocalWhiteBoard Instance;


    public Characters PlayerCharacter;
    public bool IsSinglePlayer;

    public CommonRelic PlayerCommonRelicSkillOne;
    public CommonRelic PlayerCommonRelicSkillTwo;
    public LegendaryRelic PlayerLegendarySkill;
    public AttackSkill PlayerAttackSkill;

    public Dictionary<CommonRelic, int> CommonRelicInventory = new();
    public Dictionary<LegendaryRelic, int> LegendaryRelicInventory = new();
    public int AttackLevel;

    public float Gold;

    [HideInInspector] public float GeneralVolume = 0.5f;
    [HideInInspector] public float MusicVolume = 0.5f;
    [HideInInspector] public float SFXVolume = 0.5f;
    [HideInInspector] public float AmbienceVolume = 0.5f;
    [HideInInspector] public float DialogueVolume = 0.5f;

    public static event EventHandler OnRelicEquiped;


    private void Awake() {  // Singleton, só vai ter um LocalWhiteBoard no jogo
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddToCommonDictionary(CommonRelic relic) {
        if (CommonRelicInventory.ContainsKey(relic) || relic.Character != PlayerCharacter) return;

        CommonRelicInventory.Add(relic, 1);
    }

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

    public Skill ReturnCurrentSkill (int skillIndex) {
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
}
