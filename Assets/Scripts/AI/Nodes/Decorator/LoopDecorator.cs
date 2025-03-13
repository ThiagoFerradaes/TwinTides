using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/LoopDecorator")]
public class LoopDecorator : DecoratorNode
{
    [SerializeField] bool isInfinite;
    [SerializeField, Tooltip("Esse número só é necessário se não for infinito")] int LoopAmount;
    public override Status Execute() {
        if (isInfinite) {
            Child.Execute();
            return Status.RUNNING;  
        }

        for (int i = 0; i < LoopAmount; i++) {
            Child.Execute();
        }
        return Status.SUCCESS;
    }
}
