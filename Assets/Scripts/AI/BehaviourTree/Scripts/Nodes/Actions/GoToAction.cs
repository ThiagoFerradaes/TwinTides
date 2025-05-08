using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToAction : ActionNode {

    [SerializeField] float StoppingDistance;
    Transform targetPosition;

    public override void OnStart() {

        Waypoints.PathTag tag = context.Path.Waypoints[blackboard.CurrentPathIndex];
        targetPosition = Waypoints.Instance.GetPointByTag(tag);

        context.Agent.SetDestination(targetPosition.position);

    }

    protected override State OnUpdate() {
        context.Agent.speed = context.MManager.ReturnMoveSpeed();

        if (context.Agent.pathPending) return State.Running;

        else if (context.Agent.remainingDistance > StoppingDistance) return State.Running;

        blackboard.CurrentPathIndex++;
        if (blackboard.CurrentPathIndex >= context.Path.Waypoints.Length) {
            blackboard.CurrentPathIndex = 0;
        }

        return State.Success;
    }
}
