using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/CompositeNode/Parellel")]
public class ParallelNode : CompositeNode {
    [SerializeField, Tooltip("Se verdadeiro, todos os filhos precisam retornar SUCCESS para esse nó ter sucesso, " +
        "caso falso, apenas 1 filho precisa retornar SUCCESS para o nó ter sucesso")] bool totalSuccessNode;
    Dictionary<Node, Status> _childrenStatus = new();

    public override void OnStart() {
        foreach (var child in Children) {
            _childrenStatus[child] = Status.RUNNING;
        }
    }
    public override Status Execute() {
        if (Children == null || Children.Count == 0) return Status.FAILURE;

        bool success = false;
        bool failure = false;
        bool running = false;

        foreach (var child in Children) {
            if (_childrenStatus[child] == Status.RUNNING) {

                Status tempStatus = child.Tick();

                _childrenStatus[child] = tempStatus;

                if (tempStatus == Status.SUCCESS) success = true;
                else if (tempStatus == Status.FAILURE) failure = true;
                else running = true;
            }
        }

        if (running) return Status.RUNNING;
        else if (totalSuccessNode) return failure ? Status.FAILURE : Status.SUCCESS;
        else return success ? Status.SUCCESS : Status.FAILURE;
    }

    public override void OnStop() {
        _childrenStatus.Clear();
    }
}
