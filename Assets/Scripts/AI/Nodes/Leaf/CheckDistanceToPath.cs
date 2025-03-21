using UnityEngine;

[CreateAssetMenu(menuName = "BehaviourTree/LeafNode/CheckDistanceToPath")]
public class CheckDistanceToPath : LeafNode {
    [SerializeField] float maxDistance;
    Transform closestLocation;

    public override Status Execute() {
        closestLocation = Waypoints.Instance.GetClosestPoint(Context.Path, Context.Agent.transform);

        if (Vector3.Distance(Context.Agent.transform.position, closestLocation.position) >= maxDistance) Context.Blackboard.isCloseToPath = false;
        else Context.Blackboard.isCloseToPath = true;

        return Status.SUCCESS;
    }
}
