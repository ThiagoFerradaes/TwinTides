using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDecorator : DecoratorNode
{
    [SerializeField] int amountOfLoops;
    int currentChild;

    public override void OnStart() {
        currentChild = 0;
    }

    protected override State OnUpdate() {
        if (child == null) return State.Failure;

        for (int i = currentChild; i< amountOfLoops; i++) {
            State temp = child.Update();
            if (temp == State.Failure) return State.Failure;
            else if (temp == State.Running) return State.Running;

            currentChild++;
        }

        return State.Success;
    }
}
