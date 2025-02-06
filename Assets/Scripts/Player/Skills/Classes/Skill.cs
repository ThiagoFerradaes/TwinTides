using UnityEngine;

public abstract class Skill : ScriptableObject
{
    // Atributos
    public string Name;
    public Sprite UiSprite;
    public int MinLevel, MaxLevel;
    public float Cooldown;

    // Comportamental
    [TextArea] public string[] SkillsDescriptions;
    [TextArea] public string[] UpgradesDescriptions;
    public GameObject[] skillPrefabs;

    public void UseSkill(SkillContext context, int skillLevel) {
        Debug.Log("Use Skill in Skill");

        int skillId = PlayersSkillPooling.Instance.TransformSkillInInt(this);

        PlayersSkillPooling.Instance.InstanciateObjectRpc(skillId, context, skillLevel, 0);
    }

    public string ReturnSkillDescription(int skillLevel) {
        return SkillsDescriptions[skillLevel - 1];
    }

    public string ReturnUpgradeDescription(int skillLevel) {
        return UpgradesDescriptions[skillLevel - 1];
    }
}
