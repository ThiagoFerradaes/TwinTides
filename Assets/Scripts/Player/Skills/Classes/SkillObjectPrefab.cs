using Unity.Netcode;
using UnityEngine;

public abstract class SkillObjectPrefab : NetworkBehaviour {

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOnSkillRpc(int skillId, int skillLevel, SkillContext context) {
        Debug.Log("TurnOnSKillRpc");

        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);

        ActivateSkill(skill, skillLevel, context);
    }
    public abstract void ActivateSkill(Skill info, int skillLevel, SkillContext context);

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOffSkillRpc() {
        gameObject.SetActive(false);
    }
    public void ReturnObject() {
        if (IsServer) {
            PlayerSkillPooling.Instance.ReturnObjectToPool(gameObject);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public virtual void AddStackRpc() {
        return;
    }
}
