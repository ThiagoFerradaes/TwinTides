using UnityEngine;

public class EnemyAttackPrefab : MonoBehaviour {
    protected GameObject parent;
    protected Context parentContext;
    public virtual void StartAttack(int enemyId, int skillId) {
        parent = EnemiesManager.Instance.TransformIdInEnemy(enemyId);

        if (parent.TryGetComponent<BehaviourTreeRunner>(out BehaviourTreeRunner bRunner)) {
            parentContext = bRunner.context;
        }

        EnemyAttack skill = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        if (parentContext != null && parentContext.Blackboard.Cooldowns.Count == 0) {
            foreach (var attack in skill.ListOfAttacksNames) {
                if (!parentContext.Blackboard.Cooldowns.ContainsKey(attack)) {
                    parentContext.Blackboard.Cooldowns[attack] = 0;
                }
            }
        }

    }
    public virtual void StartAttack(int enemyId, int skillId, Vector3 position) {
        parent = EnemiesManager.Instance.TransformIdInEnemy(enemyId);

        if (parent.TryGetComponent<BehaviourTreeRunner>(out BehaviourTreeRunner bRunner)) {
            parentContext = bRunner.context;
        }

        EnemyAttack skill = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        if (parentContext != null && parentContext.Blackboard.Cooldowns.Count == 0) {
            foreach (var attack in skill.ListOfAttacksNames) {
                if (!parentContext.Blackboard.Cooldowns.ContainsKey(attack)) {
                    parentContext.Blackboard.Cooldowns[attack] = 0;
                }
            }
        }
    }
    public virtual void StartAttack(int enemyId, int skillId, Vector3 position, float number) {
        parent = EnemiesManager.Instance.TransformIdInEnemy(enemyId);

        if (parent.TryGetComponent<BehaviourTreeRunner>(out BehaviourTreeRunner bRunner)) {
            parentContext = bRunner.context;
        }

        EnemyAttack skill = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        if (parentContext != null && parentContext.Blackboard.Cooldowns.Count == 0) {
            foreach (var attack in skill.ListOfAttacksNames) {
                if (!parentContext.Blackboard.Cooldowns.ContainsKey(attack)) {
                    parentContext.Blackboard.Cooldowns[attack] = 0;
                }
            }
        }
    }

    public virtual void End() {
        if (parentContext != null)
            parentContext.Blackboard.GlobalAttackTimer = parentContext.Blackboard.AttackCooldown;

        gameObject.SetActive(false);
    }
}
