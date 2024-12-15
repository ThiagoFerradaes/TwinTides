using UnityEngine;

public class NPC_Skill : Skill
{
    public override void UseSkill(SkillContext context) {
        Behaviour.UseSkill(context, LocalWhiteBoard.NPC_SKILL_INVENTORY[this]);
    }
}
