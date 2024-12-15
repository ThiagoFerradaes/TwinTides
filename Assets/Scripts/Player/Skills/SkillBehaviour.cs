using UnityEngine;

public abstract class SkillBehaviour : ScriptableObject
{
    [TextArea] public string[] SkillsDescriptions;
    [TextArea] public string[] UpgradesDescriptions;

    public abstract void UseSkill(SkillContext context, int skillLevel);

    public string ReturnSkillDescription(int skillLevel) {
        return SkillsDescriptions[skillLevel - 1];
    }

    public string ReturnUpgradeDescription(int skillLevel) {
        return UpgradesDescriptions[skillLevel - 1];
    }
}
