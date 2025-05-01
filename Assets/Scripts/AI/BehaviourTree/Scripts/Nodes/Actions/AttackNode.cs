using UnityEngine;

public class AttackNode: ActionNode
{
    [SerializeField] EnemyAttack attack;

    public override void OnStart() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(attack, 0, context.GameObject);
    }


    protected override State OnUpdate() {
        if (context.Blackboard.IsAttacking) return State.Running;

        return State.Success;
    }
}
