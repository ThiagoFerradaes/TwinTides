using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/RootNode")]
public class RootNode : Node
{
    public override Status Execute() {
        foreach(var child in Children) {
            child.Execute();
        }

        Debug.Log(name + " " + nodeStatus);
        return nodeStatus;
    }
}
