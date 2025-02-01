using UnityEngine;

[CreateAssetMenu (menuName = "Skills/CommonRelic")]
public class CommonRelic : Skill {
    public Characters Character;
    public override void UseSkill(SkillContext context) {
        Behaviour.UseSkill(context, LocalWhiteBoard.Instance.CommonRelicInventory[this]);
    }
}
