using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string Name;
    public Sprite UiSprite;
    public int MinLevel, MaxLevel;
    public SkillBehaviour Behaviour;
    public float Cooldown;

    public abstract void UseSkill(SkillContext context);
}
