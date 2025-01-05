using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalWhiteBoard : MonoBehaviour {
    public static LocalWhiteBoard Instance;


    public Characters PlayerCharacter;
    public bool IsSinglePlayer;

    public CommonRelic PlayerCommonRelicSkillOne;
    public CommonRelic PlayerCommonRelicSkillTwo;
    public NPC_Skill PlayerNpcSkillOne;
    public NPC_Skill PlayerNpcSkillTwo;
    public LegendaryRelic PlayerLegendarySkill;
    public AttackSkill PlayerAttackSkill;

    public Dictionary<CommonRelic, int> CommonRelicInventory;
    public Dictionary<LegendaryRelic, int> LegendaryRelicInventory;
    public Dictionary<NPC_Skill, int> NpcSkillInventory;
    public int AttackLevel;

    public float Gold;

    [HideInInspector] public float GeneralVolume = 0.5f;
    [HideInInspector] public float MusicVolume = 0.5f;
    [HideInInspector] public float SFXVolume = 0.5f;
    [HideInInspector] public float AmbienceVolume = 0.5f;
    [HideInInspector] public float DialogueVolume = 0.5f;


    private void Awake() {  // Singleton, só vai ter um LocalWhiteBoard no jogo
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
