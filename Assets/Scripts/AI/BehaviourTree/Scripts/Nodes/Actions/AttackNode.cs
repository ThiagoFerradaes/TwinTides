using UnityEngine;

public class AttackNode: ActionNode
{
    [SerializeField] EnemyAttack originAttack;
    EnemyAttack realAttack;

    public override void OnStart() {
        if (originAttack != null) realAttack = ScriptableObject.Instantiate(originAttack);

        realAttack.StartAttack();
    }


    protected override State OnUpdate() {
        if (realAttack.State == EnemyAttack.AttackState.RUNNING) return State.Running;

        return State.Success;
    }
}
