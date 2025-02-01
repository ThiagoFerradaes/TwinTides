using UnityEngine;

public class LegendaryRelic : Skill
{
    public override void UseSkill(SkillContext context) {
        Behaviour.UseSkill(context, LocalWhiteBoard.Instance.LegendaryRelicInventory[this]);
    }
}
