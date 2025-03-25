using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalDecorator : DecoratorNode {
    [SerializeField] Blackboard.BlackBoardBools[] conditions;
    protected override State OnUpdate() {
        if (child == null) return State.Failure;

        foreach (var tag in conditions) {
            if (!blackboard.ReturnBoolByTag(tag)) return State.Failure;
        }

        return child.Update();
    }
}
