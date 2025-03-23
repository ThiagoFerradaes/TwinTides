using System.Collections.Generic;
using UnityEngine;

public class DecoratorNode : Node
{
    public Node Child;

    public override List<Node> GetChildren() {
        return Child != null ? new List<Node> { Child } : new List<Node>();
    }

}
