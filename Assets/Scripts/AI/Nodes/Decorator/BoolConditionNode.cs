using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/Decorator/Condition/BoolCondition")]
public class BoolConditionNode : DecoratorNode {
    [SerializeField] bool isCondition;

    public override Status Execute() {
        if (Child == null) return Status.FAILURE;

        if (isCondition) {
            return Child.Execute();
        }

        return Status.FAILURE;
    }
}
