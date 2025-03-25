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
        Collider[] detectedPlayers = Physics.OverlapSphere(context.transform.position, detectionRadius, playerLayer);

        if (detectedPlayers.Length == 0) blackboard.IsTargetInRange = false;

        float closestPlayerDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var player in detectedPlayers) {
            Vector3 directionToPlayer = (player.transform.position - context.transform.position);
            float angleToPlayer = Vector3.Angle(context.transform.forward, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2) {

                if (Physics.Raycast(context.transform.position, directionToPlayer, out RaycastHit ray, detectionRadius, obstacleLayer)) {
                    blackboard.IsTargetInRange = false;
                }

                if (Physics.Raycast(context.transform.position, directionToPlayer, out ray, detectionRadius, playerLayer)) {
                    if (ray.collider != null && (ray.collider.CompareTag("Maevis") || ray.collider.CompareTag("Mel"))) {
                        if (Vector3.Distance(context.transform.position, ray.collider.transform.position) < closestPlayerDistance) {
                            closestPlayer = ray.collider.transform;
                            closestPlayerDistance = Vector3.Distance(context.transform.position, ray.collider.transform.position);
                        }
                        blackboard.Target = closestPlayer;
                        blackboard.IsTargetInRange = true;
                    }
                }

            }
        }


        return State.Success;
    }
}
