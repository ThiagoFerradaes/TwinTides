using UnityEngine;

public class FollowPlayerAction : ActionNode
{
    [SerializeField] float stoppingDistance;
    protected override State OnUpdate() {
        if (blackboard.Target == null) return State.Failure;

        if (Vector3.Distance(context.Agent.transform.position, blackboard.Target.position) >= stoppingDistance) {
            context.Agent.speed = context.MManager.ReturnMoveSpeed();
            context.Agent.SetDestination(blackboard.Target.position);
            return State.Running;
        }

        else { context.Agent.speed = 0; return State.Success; }
    }
}
