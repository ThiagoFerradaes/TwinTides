using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/RootNode")]
public class RootNode : Node {

    public DecoratorNode Child;

    public override Status Execute() {
        nodeStatus = Child.Execute();

        Debug.Log(name + " " + nodeStatus);
        return nodeStatus;
    }
}
