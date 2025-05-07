using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCooldownAction : ActionNode {
    protected override State OnUpdate() {

        if (blackboard.GlobalAttackTimer > 0) {
            blackboard.GlobalAttackTimer -= Time.deltaTime;
        }

        if (blackboard.Cooldowns.Count > 0) {
            List<string> keys = new(blackboard.Cooldowns.Keys);
            bool hasAvailableAttack = false;

            foreach (string key in keys) {
                if (blackboard.Cooldowns[key] > 0) {
                    blackboard.Cooldowns[key] -= Time.deltaTime;

                    if (blackboard.Cooldowns[key] < 0)
                        blackboard.Cooldowns[key] = 0;
                }

                if (blackboard.Cooldowns[key] == 0) {
                    hasAvailableAttack = true;
                }
            }

            if (blackboard.GlobalAttackTimer <= 0) blackboard.CanAttack = hasAvailableAttack;
        }

        return State.Running;
    }
}
