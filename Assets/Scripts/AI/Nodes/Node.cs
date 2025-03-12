using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/Node")]
public class Node : ScriptableObject
{
    public string nodeName;
    public List<Node> Children;
    public Status nodeStatus;

    public enum Status { SUCCESS, RUNNING, FAILURE};

    public virtual Status Execute() { return Status.SUCCESS; }

}
