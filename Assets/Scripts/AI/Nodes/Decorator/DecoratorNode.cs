using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/DecoratorNode")]
public class DecoratorNode : Node
{
    public Node Child;

    public override Status Execute() {
        Child.Execute();

        Debug.Log(name + " " + nodeStatus);
        return nodeStatus;
    }
}
