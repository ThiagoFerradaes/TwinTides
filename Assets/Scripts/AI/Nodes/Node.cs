using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/Node")]
public class Node : ScriptableObject
{
    public string nodeName;
    [HideInInspector] public Status nodeStatus = Status.RUNNING;

    public enum Status { SUCCESS, RUNNING, FAILURE};

    public virtual Status Execute() { return Status.SUCCESS; }

}
