using UnityEngine;

[CreateAssetMenu(menuName = "BehaviourTree/Decorator/Conditional")]
public class ConditionalDecorator : DecoratorNode
{
    [SerializeField] BlackBoard.BlackBoardBools[] conditionTags;
    [SerializeField] bool isGate;

    public override Status Execute() {
        Status temp;
        bool failure = false;

        foreach (var tag in conditionTags) {
            bool x = Context.Blackboard.ReturnBoolByTag(tag);
            if (!x) failure = true;
        }

        if (!failure) {
            temp = Child.Tick();
        }

        else {
            temp = isGate ? Status.SUCCESS : Status.FAILURE;
        }

        Debug.Log(name + " " + temp);
        return temp;
    }
}
