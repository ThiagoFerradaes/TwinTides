using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatDecorator : DecoratorNode
{
    protected override State OnUpdate() {
        if (child == null) return State.Failure;

        child.Update();

        return State.Running;
    }
}
