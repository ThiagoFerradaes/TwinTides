using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAction : ActionNode
{
    [SerializeField] string message;
    protected override State OnUpdate() {
        Debug.Log(message);

        return State.Success;
    }
}
