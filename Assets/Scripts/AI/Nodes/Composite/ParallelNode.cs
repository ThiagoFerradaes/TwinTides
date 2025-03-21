using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/CompositeNode/Parellel")]
public class ParallelNode : CompositeNode {
    [SerializeField, Tooltip("Se verdadeiro, quando um filho retorna RUNNING ele considera como SUCCESS")] bool uninterruptible;
    Dictionary<Node, Status> _childrenStatus = new();

    public override void OnStart() {
        _childrenStatus.Clear();
        foreach (var child in Children) {
            _childrenStatus[child] = Status.RUNNING;
        }
    }
    public override Status Execute() {
        if (Children == null || Children.Count == 0) return Status.FAILURE;

        bool failure = false;
        bool running = false;

        foreach (var child in Children) {
            if (_childrenStatus[child] == Status.RUNNING) {

                Status tempStatus = child.Tick();

                _childrenStatus[child] = tempStatus;

                if (tempStatus == Status.FAILURE) failure = true;
                else if(tempStatus == Status.RUNNING) running = true;
            }
        }

        if (failure) return Status.FAILURE;
        else if (!uninterruptible && running) { Debug.Log(nodeName); return Status.RUNNING; }
        else return Status.SUCCESS;
    }
}
