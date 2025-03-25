using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownAction : ActionNode
{
    [SerializeField] float cooldown;
    float currentTime = 0f;

    public override void OnStart() {
        currentTime = 0f;
    }

    protected override State OnUpdate() {

        if (currentTime < cooldown) { currentTime += Time.deltaTime; return State.Running; }

        return State.Success;
    }
}
