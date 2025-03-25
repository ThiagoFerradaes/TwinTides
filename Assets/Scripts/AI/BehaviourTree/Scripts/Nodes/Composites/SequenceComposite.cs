using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceComposite : CompositeNode {
    int currentChild;

    public override void OnStart() {
        currentChild = 0;
    }
    protected override State OnUpdate() {
        if (children == null || children.Count == 0) return State.Failure;

        for (int i = currentChild; i< children.Count; i++) {
            State temp = children[i].Update();

            if (temp == State.Running) return State.Running;

            else if (temp == State.Failure) return State.Failure;

            currentChild++;
        }

        return State.Success;
    }
}
