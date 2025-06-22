using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionAction : ActionNode
{
    [SerializeField] float detectionRadius;
    [SerializeField] float detectionAngle;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstacleLayer;

    protected override State OnUpdate() {
        if (blackboard.IsTargetForcedByCamp && blackboard.Target != null) {
            return State.Success; // Confia no que o acampamento definiu
        }

        Collider[] detectedPlayers = Physics.OverlapSphere(context.Transform.position, detectionRadius, playerLayer);
        if (detectedPlayers.Length == 0) {
            blackboard.IsTargetInRange = false;
            blackboard.Target = null;
            return State.Failure;
        }

        float closestPlayerDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var player in detectedPlayers) {
            Vector3 directionToPlayer = player.transform.position - context.Transform.position;
            float angleToPlayer = Vector3.Angle(context.Transform.forward, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2) {
                if (Physics.Raycast(context.Transform.position, directionToPlayer.normalized, out RaycastHit hit, detectionRadius)) {
                    if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0) {
                        continue; // Obstáculo no caminho
                    }

                    if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0 &&
                        (hit.collider.CompareTag("Maevis") || hit.collider.CompareTag("Mel"))) {

                        float dist = Vector3.Distance(context.Transform.position, hit.collider.transform.position);
                        if (dist < closestPlayerDistance) {
                            closestPlayer = hit.collider.transform;
                            closestPlayerDistance = dist;
                        }
                    }
                }
            }
        }

        if (closestPlayer != null) {
            blackboard.Target = closestPlayer;
            blackboard.IsTargetInRange = true;
            return State.Success;
        }

        blackboard.IsTargetInRange = false;
        blackboard.Target = null;
        return State.Failure;
    }

}
