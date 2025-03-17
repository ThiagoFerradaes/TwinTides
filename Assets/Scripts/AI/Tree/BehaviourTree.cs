using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourTree", menuName = "BehaviourTree/BehaviourTree")]
public class BehaviourTree : ScriptableObject
{
    public RootNode rootNode;
    [HideInInspector] public Node.Status treeStatus = Node.Status.RUNNING;

    public void InitiateTree(AIContext context) {
        DistributeContext(rootNode, context);       
    }

    void DistributeContext(Node node, AIContext context) {
        node.Context = context;

        if (node is LeafNode) return;

        else if (node is CompositeNode composite) foreach (var child in composite.Children) DistributeContext(child, context);

        else if (node is DecoratorNode decorator) DistributeContext(decorator.Child, context);

        else if (node is RootNode root) DistributeContext(root.Child, context);
    }
    public void ExecuteTree() {
        treeStatus = rootNode.Tick();
    }
}
