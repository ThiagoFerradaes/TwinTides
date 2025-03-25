using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelComposite : CompositeNode
{

    Dictionary<Node, State> _childrenDictionary = new();

    public override void OnStart() {
        _childrenDictionary.Clear();
        foreach(var child in children) {
            _childrenDictionary[child] = State.Running;
        }
    }

    protected override State OnUpdate() {
        if (children == null || children.Count == 0) return State.Failure;

        bool failure = false;
        bool running = false;

        foreach (var child in children) {
            if (_childrenDictionary[child] == State.Running) {
                State temp = child.Update();

                _childrenDictionary[child] = temp;

                if (temp == State.Failure) failure = true;
                else if (temp == State.Running) running = true;
            }
        }

        if (failure) return State.Failure;
        else if (running) return State.Running;
        else return State.Success;
    }
}
