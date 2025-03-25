using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalAction : ActionNode
{
    [SerializeField] Blackboard.BlackBoardBools[] conditions;

    protected override State OnUpdate() {
        foreach (var tag in conditions) {
            if (!blackboard.ReturnBoolByTag(tag)) return State.Failure;
        }

        return State.Success;
    }
}
