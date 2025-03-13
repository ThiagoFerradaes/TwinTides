using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/LoopDecorator")]
public class LoopDecorator : DecoratorNode
{
    [SerializeField] int LoopAmount;
    public override Status Execute() {
        for (int i = 0; i < LoopAmount; i++) {
            Child.Execute();
        }
        return Status.SUCCESS;
    }
}
