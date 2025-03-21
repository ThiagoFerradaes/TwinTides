using UnityEditor.Experimental.GraphView;
using UnityEngine;


[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/CompositeNode/SequenceNode")]
public class SequenceNode : CompositeNode {

    int currentChild;

    public override void OnStart() {
        currentChild = 0;
    }
    public override Status Execute() {
        if (Children == null || Children.Count == 0) return Status.FAILURE;

        for (int i = currentChild; i < Children.Count; i++) {
            Status temp = Children[i].Tick();

            if (temp == Status.RUNNING) { return Status.RUNNING; }

            else if (temp == Status.FAILURE) {
                Debug.Log(name + " FAILURE");
                return Status.FAILURE;
            }

            currentChild++;
        }

        Debug.Log(name + " SUCCESS");
        return Status.SUCCESS;
    }
}
