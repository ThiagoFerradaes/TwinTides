using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourTree", menuName = "BehaviourTree/BehaviourTree")]
public class BehaviourTree : ScriptableObject
{
    public RootNode rootNode;
    [HideInInspector] public Node.Status treeStatus = Node.Status.RUNNING;

    public void ExecuteTree() {
        treeStatus = rootNode.Execute();
    }
}
