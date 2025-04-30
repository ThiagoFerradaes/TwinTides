using System.Collections.Generic;
using UnityEngine;

public class EnemySkillConverter : MonoBehaviour {

    public static EnemySkillConverter Instance;


    // ZombieBoy
    [SerializeField] public List<EnemyAttack> zombieBoyOneOneSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyOneTwoSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyOneTreeSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyTwoOneSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyTwoTwoSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyTwoTreeSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyTreeOneSkills = new();
    [SerializeField] public List<EnemyAttack> zombieBoyTreeTwoSkills = new();

    // ZombieGirl
    [SerializeField] public List<EnemyAttack> zombieGirlOneOneSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlOneTwoSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlOneTreeSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlTwoOneSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlTwoTwoSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlTwoTreeSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlTreeOneSkills = new();
    [SerializeField] public List<EnemyAttack> zombieGirlTreeTwoSkills = new();

    // Barba Negra
    [SerializeField] public List<EnemyAttack> blackBeardSkills = new();

    List<EnemyAttack> skillsList = new();


    private void Awake() {

        if (Instance == null) {
            Instance = this;    
        }

        else {
            Destroy(this);
        }

        AddSkillToList();
    }

    void AddSkillToList() {
        foreach (var skill in zombieBoyOneOneSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieBoyOneTwoSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieBoyOneTreeSkills) { skillsList.Add(skill); }

        foreach (var skill in zombieBoyTwoOneSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieBoyTwoTwoSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieBoyTwoTreeSkills) { skillsList.Add(skill); };

        foreach (var skill in zombieBoyTreeOneSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieBoyTreeTwoSkills) { skillsList.Add(skill); }
    
        foreach (var skill in zombieGirlOneOneSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieGirlOneTwoSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieGirlOneTreeSkills) { skillsList.Add(skill); }

        foreach (var skill in zombieGirlTwoOneSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieGirlTwoTwoSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieGirlTwoTreeSkills) { skillsList.Add(skill); }

        foreach (var skill in zombieGirlTreeOneSkills) { skillsList.Add(skill); }
        foreach (var skill in zombieGirlTreeTwoSkills) { skillsList.Add(skill); }

        foreach (var skill in blackBeardSkills) { skillsList.Add(skill); }
    }
    
    public EnemyAttack TransformIdInSkill(int id) {
        return skillsList[id];
    }
    
    public int TransformSkillInInt(EnemyAttack skill) {
        return skillsList.IndexOf(skill);
    }
}
