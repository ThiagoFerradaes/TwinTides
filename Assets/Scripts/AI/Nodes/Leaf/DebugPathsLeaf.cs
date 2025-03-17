using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/LeafNode/DebugPathLeaf")]
public class DebugPathsLeaf : LeafNode
{
    public override Status Execute() {
        foreach(var text in Context.Path.Waypoints) {
            Debug.Log(text);
        }

        return Status.SUCCESS;
    }
}
