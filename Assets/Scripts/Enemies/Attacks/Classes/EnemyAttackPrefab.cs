using UnityEngine;

public class EnemyAttackPrefab : MonoBehaviour {
    protected GameObject parent;
    protected Context parentContext;
    public virtual void StartAttack(int enemyId, int skillId) {
        parent = EnemiesManager.Instance.TransformIdInEnemy(enemyId);
        parentContext = parent.GetComponent<BehaviourTreeRunner>().context;

    }

    public virtual void End() {
        gameObject.SetActive(false);
    }
}
