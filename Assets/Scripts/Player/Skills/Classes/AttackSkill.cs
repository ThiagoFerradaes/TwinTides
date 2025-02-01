using UnityEngine;

public class AttackSkill : Skill
{
    public override void UseSkill(SkillContext context) {
        Behaviour.UseSkill(context, LocalWhiteBoard.Instance.AttackLevel);
    }
}
