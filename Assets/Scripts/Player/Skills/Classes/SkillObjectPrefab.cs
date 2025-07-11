using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class SkillObjectPrefab : MonoBehaviour {

    public void TurnOnSkill(int skillId, int skillLevel, SkillContext context) {

        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);
        ActivateSkill(skill, skillLevel, context);

        StartSkillCooldown(context, skill);

        PlayersDeathManager.OnGameRestart += ReturnObject;
    }
    public abstract void ActivateSkill(Skill info, int skillLevel, SkillContext context);

    public void TurnOffSkill() {
        gameObject.SetActive(false);
    }
    public virtual void ReturnObject() {
        PlayersDeathManager.OnGameRestart -= ReturnObject;
        PlayerSkillPooling.Instance.ReturnObjectToPool(gameObject);

    }

    public virtual void AddStack() {
        return;
    }
    public virtual void StartSkillCooldown(SkillContext context, Skill skill) {
        if (skill.Character != LocalWhiteBoard.Instance.PlayerCharacter) return;
        if (skill.Character == Characters.Mel) {
            PlayerSkillPooling.Instance.MelGameObject.GetComponent<PlayerSkillManager>().StartCooldown(context.SkillIdInUI, skill);
        }
        else {
            PlayerSkillPooling.Instance.MaevisGameObject.GetComponent<PlayerSkillManager>().StartCooldown(context.SkillIdInUI, skill);
        }
    }
}
