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

        _actualTree.rootNode = CloneNode(tree.rootNode);

        _actualTree.rootNode.Execute();

        Debug.Log(_actualTree.name);
    }

    Node CloneNode(Node nodeToClone) {
        Node clonedNode = Instantiate(nodeToClone);

        clonedNode.Children = new();
        foreach(var child in nodeToClone.Children) {
            clonedNode.Children.Add(CloneNode(child));
        }

        return clonedNode;
    }
}
