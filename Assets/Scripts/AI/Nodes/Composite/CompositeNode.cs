using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/CompositeNode")]
public class CompositeNode : Node
{
    public List<Node> Children;

    public override Status Execute() {
        foreach (var child in Children) {
            child.Execute();
        }

        Debug.Log(name + " " + nodeStatus);
        return nodeStatus;
    }
}
