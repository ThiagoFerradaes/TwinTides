using UnityEngine;

public class EnemyAttackPrefab : MonoBehaviour {
    protected GameObject parent;
    protected Context parentContext;
    public virtual void StartAttack(int enemyId, int skillId) {
        parent = EnemiesManager.Instance.TransformIdInEnemy(enemyId);
        parentContext = parent.GetComponent<BehaviourTreeRunner>().context;

        EnemyAttack skill = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        if (parentContext.Blackboard.Cooldowns.Count == 0) {
            foreach (var attack in skill.ListOfAttacksNames) {
                if (!parentContext.Blackboard.Cooldowns.ContainsKey(attack)) {
                    parentContext.Blackboard.Cooldowns[attack] = 0;
                }
            }
        }

    }
    public virtual void StartAttack(int enemyId, int skillId, Vector3 position) {
        parent = EnemiesManager.Instance.TransformIdInEnemy(enemyId);
        parentContext = parent.GetComponent<BehaviourTreeRunner>().context;

    }

    public virtual void End() {
        parentContext.Blackboard.GlobalAttackTimer = parentContext.Blackboard.AttackCooldown;
        gameObject.SetActive(false);
    }
}
