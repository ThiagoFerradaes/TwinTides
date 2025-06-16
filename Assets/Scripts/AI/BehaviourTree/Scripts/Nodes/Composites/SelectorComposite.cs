using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorComposite : CompositeNode
{
    int currentChild;

    public override void OnStart() {
        currentChild = 0;
    }

    protected override State OnUpdate() {
        if (children == null || children.Count == 0) return State.Failure;
        
        for (int i = currentChild; i < children.Count; i++) {
            State temp = children[i].Update();
            if (temp == State.Running) {
                for (int j = currentChild + 1; j < children.Count; j++) {
                    if (children[j].started) {
                        children[j].Abort();
                    }
                }
                return State.Running;
            }
            else if (temp == State.Success) {
                for (int j = currentChild + 1; j < children.Count; j++) {
                    if (children[j].started) {
                        children[j].Abort();
                    }
                }
                return State.Success;
            }

            currentChild++;
        }

        return State.Failure;
    }
}
