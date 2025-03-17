using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public string nodeName;
    [HideInInspector] public Status NodeStatus = Status.SUCCESS;

    public enum Status { SUCCESS, RUNNING, FAILURE};

    public Status Tick() {
        if (NodeStatus != Status.RUNNING) OnStart();

        NodeStatus = Execute();

        if (NodeStatus != Status.RUNNING) OnStop();

        return NodeStatus;
    }


    public virtual void OnStart() { }
    public virtual Status Execute() { return Status.SUCCESS; }
    public virtual void OnStop() { }

}
