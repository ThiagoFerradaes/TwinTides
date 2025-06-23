using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;

public class AttackNode : ActionNode {
    [SerializeField] EnemyAttack attack;
    [SerializeField] bool animated;
    [SerializeField] string AnimationTriggerName;
    [SerializeField] string AnimationName;
    [SerializeField] float animationPercent;

    public override void OnStart() {
        if (!animated) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(attack, 0, context.GameObject);
        }
        else {
            context.GameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackRoutine());
        }
    }

    protected override State OnUpdate() {
        if (context.Blackboard.IsAttacking) return State.Running;

        return State.Success;
    }

    IEnumerator AttackRoutine() {
        Debug.Log("Entered coroutine");
        context.Blackboard.IsAttacking = true;

        context.Anim.SetTrigger(AnimationTriggerName);

        AnimatorStateInfo stateInfo = context.Anim.GetCurrentAnimatorStateInfo(0);

        while (context.Anim.IsInTransition(0))
            yield return null;

        while (!stateInfo.IsName(AnimationName)) {
            yield return null;
            stateInfo = context.Anim.GetCurrentAnimatorStateInfo(0);
        }

        while (stateInfo.normalizedTime < animationPercent) {
            yield return null;
            stateInfo = context.Anim.GetCurrentAnimatorStateInfo(0);
        }

        EnemySkillPooling.Instance.RequestInstantiateAttack(attack, 0, context.GameObject);

    }
}

