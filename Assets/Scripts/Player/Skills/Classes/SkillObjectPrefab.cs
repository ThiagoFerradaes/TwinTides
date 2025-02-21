using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class SkillObjectPrefab : NetworkBehaviour {

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOnSkillRpc(int skillId, int skillLevel, SkillContext context) {

        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);

        ActivateSkill(skill, skillLevel, context);

        StartSkillCooldown(context, skill);
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
    public virtual void StartSkillCooldown(SkillContext context, Skill skill) {
        if (skill.Character == Characters.Mel) {
            PlayerSkillPooling.Instance.MelGameObject.GetComponent<PlayerSkillManager>().StartCooldown(context.SkillIdInUI, skill);
        }
        else {
            PlayerSkillPooling.Instance.MaevisGameObject.GetComponent<PlayerSkillManager>().StartCooldown(context.SkillIdInUI, skill);
        }
    }
}
