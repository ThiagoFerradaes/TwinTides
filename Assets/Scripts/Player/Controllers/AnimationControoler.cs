using Unity.Netcode;
using UnityEngine;

public class AnimationControoler : NetworkBehaviour
{
    Animator anim;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        anim = GetComponentInChildren<Animator>();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void HandleAnimationRpc(bool isTrigger, string animationTriggerName, bool isTrue) {
        if (isTrigger) anim.SetTrigger(animationTriggerName);
        else anim.SetBool(animationTriggerName, isTrue);
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void HandleAnimationSpeedRpc(string parameterName, float speed) {
        anim.SetFloat(parameterName, speed);
    }
}
