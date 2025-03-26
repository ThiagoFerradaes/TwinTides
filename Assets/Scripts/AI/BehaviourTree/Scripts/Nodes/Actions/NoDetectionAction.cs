using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDetectionAction : ActionNode
{

    protected override State OnUpdate() {
        blackboard.CanFollowPlayer = false;
        blackboard.IsTargetInRange = false;

        return State.Success;
    }
}
