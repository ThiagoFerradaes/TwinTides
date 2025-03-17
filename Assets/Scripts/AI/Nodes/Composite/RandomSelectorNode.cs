using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/CompositeNode/RandomSelector")]
public class RandomSelectorNode : CompositeNode {
    public override Status Execute() {
        if (Children == null || Children.Count == 0) return Status.FAILURE;

        int rng = Random.Range(0, Children.Count);

        Status temp = Children[rng].Tick();

        Debug.Log(name + " " + temp);
        return temp;
    }
}
