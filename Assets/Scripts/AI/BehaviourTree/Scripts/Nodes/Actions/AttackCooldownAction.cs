using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCooldownAction : ActionNode
{

    protected override State OnUpdate() {
        if (blackboard.AttackCooldown > 0) {
            blackboard.CanAttack = false;
            blackboard.AttackCooldown -= Time.deltaTime;
            return State.Running;
        }

        blackboard.CanAttack = true;

        return State.Success;
    }
}
