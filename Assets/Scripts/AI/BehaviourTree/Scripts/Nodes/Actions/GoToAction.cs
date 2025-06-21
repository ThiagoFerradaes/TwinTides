using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToAction : ActionNode {

    [SerializeField] float StoppingDistance;
    Transform targetPosition;

    public override void OnStart() {

        if (blackboard.path == null || blackboard.path.Count == 0) {
            state = State.Failure;
            return;
        }

        // Garante que o índice está dentro do intervalo
        if (blackboard.CurrentPathIndex >= blackboard.path.Count) {
            blackboard.CurrentPathIndex = 0;
        }

        targetPosition = blackboard.path[blackboard.CurrentPathIndex];
        context.Agent.SetDestination(targetPosition.position);

        if (context.anim != null && context.anim.enabled) context.anim.SetBool("IsWalking", true);
    }

    protected override State OnUpdate() {
        if (state == State.Failure) {
            return State.Failure;
        }

        context.Agent.speed = context.MManager.ReturnMoveSpeed();

        if (context.Agent.pathPending) return State.Running;

        if (context.Agent.remainingDistance > StoppingDistance) return State.Running;

        // Avança para o próximo ponto
        blackboard.CurrentPathIndex++;
        if (blackboard.CurrentPathIndex >= blackboard.path.Count) {
            blackboard.CurrentPathIndex = 0;
        }

        return State.Success;
    }

    public override void OnStop() {
        if (context.anim != null && context.anim.enabled) {
            context.anim.SetBool("IsWalking", false);
        }
    }
}
