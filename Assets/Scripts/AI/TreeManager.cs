using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour {
    [SerializeField] BehaviourTree tree;
    BehaviourTree _actualTree;

    private void Start() {
        CloneTree();
    }

    void CloneTree() {
        _actualTree = Instantiate(tree);

        _actualTree.rootNode = (RootNode)CloneNode(tree.rootNode);
    }

    Node CloneNode(Node nodeToClone) {
        Node clonedNode = Instantiate(nodeToClone);

        if (clonedNode is RootNode root) {
            RootNode newNode = (RootNode)clonedNode;

            if (newNode.Child != null) root.Child = (DecoratorNode)CloneNode(newNode.Child);
        }

        else if (clonedNode is DecoratorNode decorator) {
            DecoratorNode newNode = (DecoratorNode)clonedNode;

            if (newNode.Child != null) decorator.Child = CloneNode(newNode.Child);
        }

        else if (clonedNode is CompositeNode compositeNode) {
            CompositeNode newNode = (CompositeNode)clonedNode;

            if (newNode.Children != null) {
                foreach (var child in newNode.Children) {
                    compositeNode.Children.Add(CloneNode(child));
                }
            }
        }

        return clonedNode;
    }

    private void Update() {
        if (_actualTree.treeStatus == Node.Status.RUNNING) _actualTree.ExecuteTree();
    }
}
