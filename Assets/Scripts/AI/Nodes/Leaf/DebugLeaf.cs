using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/DebugLeaf")]
public class DebugLeaf : LeafNode
{
    [SerializeField] string debug;

    public override Status Execute() {
        Debug.Log(debug);
        return Status.SUCCESS;
    }
}
