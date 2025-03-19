using UnityEngine;

[CreateAssetMenu(menuName = "BehaviourTree/CompositeNode/Selector")]
public class SelectorComposite : CompositeNode
{
    int currentChild;

    public override void OnStart() {
        currentChild = 0;
    }
    public override Status Execute() {
        for (int i = currentChild; i < Children.Count; i++) { 
            Status temp = Children[i].Tick();

            if (temp == Status.RUNNING) return Status.RUNNING;

            else if (temp == Status.SUCCESS) { currentChild++; return Status.SUCCESS; }

            else { currentChild++; }
        }

        return Status.FAILURE;
    }
}
