using UnityEngine;

public abstract class EnemyAttack : ScriptableObject {

    public enum AttackState { RUNNING, SUCCESS }

    protected AttackState state = AttackState.RUNNING;

    public AttackState State => state;

    public abstract void StartAttack();

    public abstract void UpdateAttack();
}
