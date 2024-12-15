using System.Collections.Generic;
using UnityEngine;

public static class LocalWhiteBoard 
{
    public static Characters PLAYER_CHARACTER;

    public static ComonRelic PLAYER_COMMON_RELIC_SKILL_ONE;
    public static ComonRelic PLAYER_COMMON_RELIC_SKILL_TWO;
    public static NPC_Skill PLAYER_NPC_SKILL_ONE;
    public static NPC_Skill PLAYER_NPC_SKILL_TWO;
    public static LegendaryRelic PLAYER_LEGENDARY_SKILL;
    public static AttackSkill PLAYER_ATTACK_SKILL;

    public static Dictionary<ComonRelic, int> COMMON_RELIC_INVENTORY;
    public static Dictionary<LegendaryRelic, int> LEGENDARY_RELIC_INVENTORY;
    public static Dictionary<NPC_Skill, int> NPC_SKILL_INVENTORY;
    public static int ATTACK_LEVEL;
}
