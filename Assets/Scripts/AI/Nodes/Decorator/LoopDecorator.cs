using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/Decorator/LoopDecorator")]
public class LoopDecorator : DecoratorNode
{
    [SerializeField] bool isInfinite;
    [SerializeField, Tooltip("Esse número só é necessário se não for infinito")] int LoopAmount;
    public override Status Execute() {
        if (Child == null) return Status.FAILURE;

        Status temp = Status.SUCCESS;

        if (isInfinite) {
            Child.Tick();
            return Status.RUNNING;  
        }

        for (int i = 0; i < LoopAmount; i++) {
            temp = Child.Tick();
        }

        Debug.Log(name + " " +  temp);

        return temp;
    }
}
