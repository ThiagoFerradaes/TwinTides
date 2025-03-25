using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerAction : ActionNode
{
    [SerializeField] float stoppingDistance;
    protected override State OnUpdate() {
        if (blackboard.Target == null) return State.Failure;

        if (Vector3.Distance(context.agent.transform.position, blackboard.Target.position) >= stoppingDistance) {
            context.agent.speed = context.MManager.ReturnMoveSpeed();
            context.agent.SetDestination(blackboard.Target.position);
            return State.Running;
        }

        else { context.agent.speed = 0; return State.Success; }
    }
}
