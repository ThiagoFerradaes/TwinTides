using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToAction : ActionNode {
    [SerializeField] Waypoints.PathTag targetLocation;
    [SerializeField] float StoppingDistance;
    Transform targetPosition;

    public override void OnStart() {
        if (targetPosition == null) targetPosition = Waypoints.Instance.GetPointByTag(targetLocation);
        context.agent.SetDestination(targetPosition.position);
    }

    protected override State OnUpdate() {
        //Context.Agent.speed = Context.MManager.ReturnMoveSpeed();

        if (context.agent.pathPending) return State.Running;

        else if (context.agent.remainingDistance > StoppingDistance) return State.Running;

        return State.Success;
    }
}
