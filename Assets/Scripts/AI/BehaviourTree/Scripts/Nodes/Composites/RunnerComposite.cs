using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerComposite : CompositeNode
{

    protected override State OnUpdate() {
        if (children == null || children.Count == 0) return State.Failure;

        foreach (var child in children) {
            State temp = child.Update();

            if (temp == State.Failure) return State.Failure;
        }

        return State.Success;
    }
}
