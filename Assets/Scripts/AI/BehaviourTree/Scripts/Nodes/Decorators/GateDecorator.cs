using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateDecorator : DecoratorNode
{
    [SerializeField] Blackboard.BlackBoardBools[] conditions;
    bool running;
    protected override State OnUpdate() {
        if (child == null) return State.Failure;

        foreach (var tag in conditions) {
            if (!blackboard.ReturnBoolByTag(tag)) return State.Success;
        }

        running = true;

        return child.Update();
    }

    public override void OnStop() {
        if (running) { running = false; child.Abort(); }
    }
}
