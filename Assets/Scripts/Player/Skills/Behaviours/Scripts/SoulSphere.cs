using UnityEngine;

[CreateAssetMenu(menuName = "SkillBehaviour/SoulSphere")]
public class SoulSphere : SkillBehaviour {
    public override void UseSkill(SkillContext context, int skillLevel) {
        Debug.Log("SoulSphere used");
    }
}
