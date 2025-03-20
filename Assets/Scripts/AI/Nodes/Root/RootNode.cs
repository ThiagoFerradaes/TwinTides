using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/RootNode")]
public class RootNode : Node {

    public DecoratorNode Child;

    public override Status Execute() {
        if (Child == null) return Status.FAILURE;

        Status temp = Child.Tick();

        return temp;
    }
}
