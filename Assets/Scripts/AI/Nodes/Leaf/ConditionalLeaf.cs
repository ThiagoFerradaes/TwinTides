using UnityEngine;

[CreateAssetMenu (menuName = "BehaviourTree/LeafNode/Conditional")]
public class ConditionalLeaf : LeafNode {
    [SerializeField] BlackBoard.BlackBoardBools conditionTag;

    public override Status Execute() {
        if (Context.Blackboard.ReturnBoolByTag(conditionTag)) {
            return Status.SUCCESS;
        }
        else {
            return Status.FAILURE;
        }
    }
}
