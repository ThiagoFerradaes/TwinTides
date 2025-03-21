using UnityEngine;

[CreateAssetMenu (menuName = "BehaviourTree/LeafNode/FollowTarget")]
public class FollowTargetLeaf : LeafNode
{
    [SerializeField] float stoppingDistance;
    public override Status Execute() {
        if (Context.Blackboard.Target == null) return Status.FAILURE;

        if (Vector3.Distance(Context.Agent.transform.position, Context.Blackboard.Target.position) >= stoppingDistance) {
            Context.Agent.speed = Context.MManager.ReturnMoveSpeed();
            Context.Agent.SetDestination(Context.Blackboard.Target.position);
            return Status.RUNNING;
        }

        else { Context.Agent.speed = 0; return Status.SUCCESS; }
    }
}
