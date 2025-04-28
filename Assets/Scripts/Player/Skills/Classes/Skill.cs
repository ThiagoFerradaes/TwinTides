using UnityEngine;

public abstract class Skill : ScriptableObject
{
    // Atributos
    public string Name;
    public Sprite UiSprite;
    public Sprite NameSpriteTotem;
    public int MinLevel, MaxLevel;
    public float Cooldown;
    public bool IsStackable = true;
    public Characters Character;

    // Comportamental
    [TextArea] public string[] SkillsDescriptions;
    [TextArea] public string[] UpgradesDescriptions;
    public GameObject[] skillPrefabs;

    public void UseSkill(SkillContext context, int skillLevel) {

        Debug.Log("Skill used");

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(this);

        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, context, skillLevel, 0);

    }

    public string ReturnSkillDescription(int skillLevel) {
        return SkillsDescriptions[skillLevel - 1];
    }

    public string ReturnUpgradeDescription(int skillLevel) {
        return UpgradesDescriptions[skillLevel - 1];
    }

    public virtual float ReturnCooldown() {
        return Cooldown;
    }
}
