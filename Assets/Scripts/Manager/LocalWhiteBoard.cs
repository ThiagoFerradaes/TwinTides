using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalWhiteBoard : MonoBehaviour
{
    public static LocalWhiteBoard Instance;


    public Characters PlayerCharacter;

    public ComonRelic PlayerCommonRelicSkillOne;
    public ComonRelic PlayerCommonRelicSkillTwo;
    public NPC_Skill PlayerNpcSkillOne;
    public NPC_Skill PlayerNpcSkillTwo;
    public LegendaryRelic PlayerLegendarySkill;
    public AttackSkill PlayerAttackSkill;

    public Dictionary<ComonRelic, int> CommonRelicInventory;
    public Dictionary<LegendaryRelic, int> LegendaryRelicInventory;
    public Dictionary<NPC_Skill, int> NpcSkillInventory;
    public int AttackLevel;

    public float Gold;


    private void Awake() {  // Singleton, só vai ter um LocalWhiteBoard no jogo
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
