using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "BehaviourTree/LeafNode/CooldownLeaf")]
public class CooldownLeaf : LeafNode
{
    [SerializeField] float cooldownTime;
    float _currentTimer;

    public override void OnStart() {
        _currentTimer = 0f;
    }

    public override Status Execute() {
        _currentTimer += Time.deltaTime;

        if (_currentTimer >= cooldownTime) return Status.SUCCESS;
        else return Status.RUNNING;
    }
}
