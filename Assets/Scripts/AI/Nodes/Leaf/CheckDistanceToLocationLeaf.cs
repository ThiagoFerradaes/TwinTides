using UnityEngine;

[CreateAssetMenu(menuName = "BehaviourTree/LeafNode/CheckDistanceToLocation")]
public class CheckDistanceToLocationLeaf : LeafNode
{
    [SerializeField] Waypoints.PathTag locationToCheckDistanceTag;
    [SerializeField] float maxDistanceToLocation;
    Transform location;

    public override void OnStart() {
        if (location == null) location = Waypoints.Instance.GetPointByTag(locationToCheckDistanceTag);
    }
    public override Status Execute() {
        if (Vector3.Distance(Context.Agent.transform.position, location.position) >= maxDistanceToLocation) return Status.FAILURE;

        return Status.SUCCESS;
    }
}
