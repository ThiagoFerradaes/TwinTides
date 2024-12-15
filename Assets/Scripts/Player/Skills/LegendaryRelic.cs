using UnityEngine;

public class LegendaryRelic : Skill
{
    public override void UseSkill(SkillContext context) {
        Behaviour.UseSkill(context, LocalWhiteBoard.LEGENDARY_RELIC_INVENTORY[this]);
    }
}
