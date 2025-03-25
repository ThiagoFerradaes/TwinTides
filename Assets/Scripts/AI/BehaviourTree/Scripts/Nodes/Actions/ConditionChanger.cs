using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionChanger : ActionNode
{
    [SerializeField] Blackboard.BlackBoardBools[] conditions;
    [SerializeField] bool changeToTrue;
    protected override State OnUpdate() {
        foreach (var condition in conditions) {
            blackboard.ReturnRefByTag(condition) = changeToTrue;
        }

        return State.Success;
    }
}
