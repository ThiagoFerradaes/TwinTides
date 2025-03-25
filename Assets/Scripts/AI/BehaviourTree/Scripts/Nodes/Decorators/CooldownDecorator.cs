using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownDecorator : DecoratorNode
{
    [SerializeField] float cooldown;
    float currentTime = 0;

    public override void OnStart() {
        currentTime = 0;
    }
    protected override State OnUpdate() {
        if (child == null) return State.Failure;

        if (currentTime < cooldown) {
            currentTime += Time.deltaTime;
            return State.Running;
        }

        return child.Update();
    }
}
