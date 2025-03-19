using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreeManager : MonoBehaviour {
    [SerializeField] BehaviourTree tree;
    [SerializeField] AIPath path;
    BehaviourTree _actualTree;

    private void Start() {
        CloneTree();
    }

    void CloneTree() {
        _actualTree = Instantiate(tree);

        _actualTree.rootNode = (RootNode)CloneNode(tree.rootNode);

        AIContext context = new(path, GetComponent<NavMeshAgent>(), GetComponent<MovementManager>());

        _actualTree.InitiateTree(context);
    }

    Node CloneNode(Node nodeToClone) {
        Node clonedNode = Instantiate(nodeToClone);

        if (clonedNode is RootNode root) {

            if (root.Child != null) root.Child = (DecoratorNode)CloneNode(root.Child);
        }

        else if (clonedNode is DecoratorNode decorator) {

            if (decorator.Child != null) decorator.Child = CloneNode(decorator.Child);
        }

        else if (clonedNode is CompositeNode newNode) {

            if (newNode.Children != null && newNode.Children.Count > 0) {

                List<Node> clonedChildren = new();

                for (int i = 0; i < newNode.Children.Count; i++) {
                    clonedChildren.Add(CloneNode(newNode.Children[i]));
                }

                newNode.Children = clonedChildren;
            }
        }

        return clonedNode;
    }

    private void Update() {
        if (_actualTree.treeStatus == Node.Status.RUNNING) _actualTree.ExecuteTree();
    }
}
