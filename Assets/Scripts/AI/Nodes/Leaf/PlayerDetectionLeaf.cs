using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/LeafNode/PlayerDetection")]
public class PlayerDetectionLeaf : LeafNode
{
    [SerializeField] float DetectionRadius;
    [SerializeField] float DetectionAngle;
    [SerializeField] LayerMask DetectionLayer;
    [SerializeField] LayerMask ObstacleLayer;


    public override Status Execute() {
        Collider[] detectedPlayers = Physics.OverlapSphere(Context.Agent.transform.position, DetectionRadius, DetectionLayer);

        if (detectedPlayers.Length == 0) Context.Blackboard.isTargetInRange = false;

        float closestPlayerDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var player in detectedPlayers) {
            Vector3 directionToPlayer = (player.transform.position - Context.Agent.transform.position);
            float angleToPlayer = Vector3.Angle(Context.Agent.transform.forward, directionToPlayer);

            if (angleToPlayer <= DetectionAngle / 2) {

                if (Physics.Raycast(Context.Agent.transform.position, directionToPlayer, out RaycastHit ray, DetectionRadius,ObstacleLayer)) {
                    Context.Blackboard.isTargetInRange = false;
                }

                if (Physics.Raycast(Context.Agent.transform.position, directionToPlayer, out ray, DetectionRadius, DetectionLayer)) {
                    if (ray.collider != null && (ray.collider.CompareTag("Maevis") || ray.collider.CompareTag("Mel"))) {
                        if (Vector3.Distance(Context.Agent.transform.position, ray.collider.transform.position) < closestPlayerDistance) {
                            closestPlayer = ray.collider.transform;
                            closestPlayerDistance = Vector3.Distance(Context.Agent.transform.position, ray.collider.transform.position);
                        }
                        Context.Blackboard.Target = closestPlayer;
                        Context.Blackboard.isTargetInRange = true;
                    }
                }

            }
        }

        return Status.SUCCESS;
    }
}
