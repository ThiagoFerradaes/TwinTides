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

        if (detectedPlayers.Length == 0) Debug.Log("Nothing Found");

        foreach (var player in detectedPlayers) {
            Vector3 directionToPlayer = (player.transform.position - Context.Agent.transform.position);
            float angleToPlayer = Vector3.Angle(Context.Agent.transform.forward, directionToPlayer);

            if (angleToPlayer <= DetectionAngle / 2) {

                if (Physics.Raycast(Context.Agent.transform.position, directionToPlayer, out RaycastHit ray, DetectionRadius,ObstacleLayer)) {
                    Debug.Log("Wall Found");
                    return Status.FAILURE;
                }

                if (Physics.Raycast(Context.Agent.transform.position, directionToPlayer, out ray, DetectionRadius, DetectionLayer)) {
                    if (ray.collider != null && (ray.collider.CompareTag("Maevis") || ray.collider.CompareTag("Mel"))) {
                        Debug.Log("Player Found");
                        return Status.SUCCESS;
                    }
                }

            }
        }

        return Status.FAILURE;
    }
}
