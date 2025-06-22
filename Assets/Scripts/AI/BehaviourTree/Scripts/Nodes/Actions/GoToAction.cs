using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToAction : ActionNode {

    [SerializeField] float StoppingDistance;
    Transform targetPosition;

    public override void OnStart() {

        if (blackboard.originPoint == null) {
            state = State.Failure;
            return;
        }

        targetPosition = blackboard.originPoint;
        context.Agent.SetDestination(targetPosition.position);

        if (context.anim != null && context.anim.enabled) {
            context.anim.SetBool("IsWalking", true);
        }
    }

    protected override State OnUpdate() {
        if (state == State.Failure) {
            return State.Failure;
        }

        context.Agent.speed = context.MManager.ReturnMoveSpeed();

        if (context.Agent.pathPending) return State.Running;

        if (context.Agent.remainingDistance > StoppingDistance) return State.Running;

        return State.Success;
    }

    public override void OnStop() {
        if (context.anim != null && context.anim.enabled) {
            context.anim.SetBool("IsWalking", false);
        }
    }
}

