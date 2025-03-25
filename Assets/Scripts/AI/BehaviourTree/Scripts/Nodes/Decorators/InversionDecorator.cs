using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InversionDecorator : DecoratorNode
{

    protected override State OnUpdate() {
        if (child == null) return State.Failure;

        State temp = child.Update();

        if (temp == State.Running) return State.Running;
        else if (temp == State.Failure) return State.Success;
        else return State.Failure;
    }
}
