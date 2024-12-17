using UnityEngine;

public class ComonRelic : Skill {
    public override void UseSkill(SkillContext context) {
        Behaviour.UseSkill(context, LocalWhiteBoard.Instance.CommonRelicInventory[this]);
    }
}
