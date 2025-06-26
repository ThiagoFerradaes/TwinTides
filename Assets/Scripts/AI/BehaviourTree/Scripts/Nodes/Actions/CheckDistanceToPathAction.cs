using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceToPathAction : ActionNode
{
    protected override State OnUpdate() {
        if (blackboard.TargetInsideCamp) {
            blackboard.IsCloseToPath = true;
            return State.Success;
        }

        if (blackboard.originPoint == null) {
            blackboard.IsCloseToPath = true; 
            return State.Success;
        }

        blackboard.IsCloseToPath = Vector3.Distance(context.Agent.transform.position, blackboard.originPoint.position) < blackboard.maxDistanceToPath;

        return State.Success;
    }
}
