using Unity.Netcode;
using UnityEngine;

public abstract class SkillObjectPrefab : NetworkBehaviour
{
    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOnSkillRpc(int skillId, int skillLevel, SkillContext context) {
        Debug.Log("TurnOnSKillRpc");

        Skill skill = PlayersSkillPooling.Instance.TransformIdInSkill(skillId);

        ActivateSkill(skill, skillLevel, context);
    }
    public abstract void ActivateSkill(Skill info, int skillLevel, SkillContext context);

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOffSkillRpc() {
        gameObject.SetActive(false);
    }

    public void Test(int skillId, int skillLevel, SkillContext context) {
        TurnOnSkillRpc(skillId, skillLevel, context);
    }
}
