using UnityEngine;

[CreateAssetMenu(menuName = "SkillBehaviour/GhostlyWhispers")]
public class GhostlyWhispers : SkillBehaviour {
    public override void UseSkill(SkillContext context, int skillLevel) {
        Debug.Log("GhostlyWhispers");
    }
}
