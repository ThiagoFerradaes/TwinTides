using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/Decorator/Inversion")]
public class InversionDecorator : DecoratorNode {
    public override Status Execute() {
        Status temp = Child.Tick();

        if (temp == Status.SUCCESS) return Status.FAILURE;
        else if (temp == Status.FAILURE) return Status.SUCCESS;
        else return Status.RUNNING;
    }
}
