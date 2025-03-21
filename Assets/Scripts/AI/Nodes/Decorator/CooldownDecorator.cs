using UnityEngine;

[CreateAssetMenu(menuName = "BehaviourTree/Decorator/Cooldown")]
public class CooldownDecorator : DecoratorNode
{
    [SerializeField] float cooldown;
    float _currentTime;

    public override void OnStart() {
        _currentTime = 0;
    }

    public override Status Execute() {
        _currentTime += Time.deltaTime;

        if (_currentTime >= cooldown) { Status temp = Child.Tick(); return temp; }

        return Status.RUNNING;
    }
}
