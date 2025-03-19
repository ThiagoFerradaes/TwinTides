using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/LeafNode/GoToNode")]
public class GoToLeafNode : LeafNode
{
    [SerializeField] Waypoints.PathTag targetLocation;
    [SerializeField] float StoppingDistance;
    Transform targetPosition;

    public override void OnStart() {
        if (targetPosition == null) targetPosition = Waypoints.Instance.GetPointByTag(targetLocation);
        Context.Agent.SetDestination(targetPosition.position);
    }
    public override Status Execute() {
        Context.Agent.speed = Context.MManager.ReturnMoveSpeed();
        if (Context.Agent.pathPending) return Status.RUNNING;

        else if (Context.Agent.remainingDistance > StoppingDistance) { return Status.RUNNING; }

        return Status.SUCCESS;
    }
}
