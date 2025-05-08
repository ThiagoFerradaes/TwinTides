using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkillConverter : MonoBehaviour {
    public static PlayerSkillConverter Instance;

    // Mel
    [SerializeField] List<Skill> melCommonSkillIndex = new();
    [SerializeField] List<Skill> melLegendarySkillIndex = new();
    [SerializeField] List<Skill> melAtackSkillIndex = new();

    // Maevis
    [SerializeField] List<Skill> maevisCommonSkillIndex = new();
    [SerializeField] List<Skill> maevisLegendarySkillIndex = new();
    [SerializeField] List<Skill> maevisAtackSkillIndex = new();
    private List<Skill> skillIndex = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
        AddSKillsToList();
    }
    void AddSKillsToList() {
        foreach (var skill in melCommonSkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in melLegendarySkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in melAtackSkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in maevisCommonSkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in maevisLegendarySkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in maevisAtackSkillIndex) {
            skillIndex.Add(skill);
        }
    }
    public int TransformSkillInInt(Skill skill) {
        return skillIndex.IndexOf(skill);
    }

    public Skill TransformIdInSkill(int skillId) {
        return skillIndex[skillId];
    }

    public List<Skill> ReturnCommonSkillList(Characters character) {
        List<Skill> newList = character == Characters.Maevis ? maevisCommonSkillIndex : melCommonSkillIndex;
        return newList;
    }
    public List<Skill> ReturnLegendarySkillList(Characters character) {
        List<Skill> newList = character == Characters.Maevis ? maevisLegendarySkillIndex : melLegendarySkillIndex;
        return newList;
    }
}
