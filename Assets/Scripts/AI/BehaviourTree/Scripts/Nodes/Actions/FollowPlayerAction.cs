using UnityEngine;

public class FollowPlayerAction : ActionNode {
    [SerializeField] float minRange;
    bool isRetreating;
    bool canRetreat = true;
    Vector3 retreatPosition;
    protected override State OnUpdate() {
        if (blackboard.Target == null) return State.Failure;

        Vector3 direction = (blackboard.Target.position - context.Agent.transform.position);
        direction.y = 0;

        float distance = Vector3.Distance(context.Agent.transform.position, blackboard.Target.position);

        if (distance >= blackboard.AttackRange && canRetreat) { // Perguntando se o inimigo já esta perto o suficiente do jogador
            context.Agent.speed = context.MManager.ReturnMoveSpeed();
            context.Agent.SetDestination(blackboard.Target.position);
            if (context.Anim != null && context.Anim.enabled) {
                context.Anim.SetBool("IsWalking", true);
            }
            blackboard.IsInAttackRange = false;
            return State.Running;
        }

        else {
            if (distance < minRange && !isRetreating && canRetreat) {
                Vector3 retreatDirection = (context.Agent.transform.position - blackboard.Target.position).normalized;
                retreatPosition = blackboard.Target.position + retreatDirection * blackboard.AttackRange;
                isRetreating = true;
                canRetreat = false;
                if (context.Anim != null && context.Anim.enabled) context.Anim.SetBool("IsWalking", true);
                return State.Running;
            }
            else if (isRetreating) {
                context.Agent.updateRotation = false; // impede que ele vire

                context.Agent.speed = context.MManager.ReturnMoveSpeed();
                context.Agent.SetDestination(retreatPosition);
                if (context.Anim != null && context.Anim.enabled) context.Anim.SetBool("IsWalking", true);

                Quaternion targetRotation = Quaternion.LookRotation(blackboard.Target.position - context.Agent.transform.position);
                context.Agent.transform.rotation = Quaternion.RotateTowards(
                    context.Agent.transform.rotation,
                    targetRotation,
                    context.Agent.angularSpeed * Time.deltaTime
                );

                if (!context.Agent.pathPending && context.Agent.remainingDistance <= context.Agent.stoppingDistance) {
                    isRetreating = false;
                    if (context.Anim != null && context.Anim.enabled) {
                        context.Anim.SetBool("IsWalking", false);
                    }
                }

                blackboard.IsInAttackRange = false;
                return State.Running;

            }
            
            else {
                context.Agent.speed = 0;
                context.Agent.ResetPath();
                context.Agent.velocity = Vector3.zero; // Esses três parecem ser necessários para parar completamente a movimentação do inimigo

                if (context.Anim != null && context.Anim.enabled) {
                    context.Anim.SetBool("IsWalking", false);
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                if (direction.sqrMagnitude >= 0.1f) { // forçando o inimigo a virar para o jogador
                    context.Agent.transform.rotation = Quaternion.RotateTowards(context.Agent.transform.rotation, targetRotation, context.Agent.angularSpeed * Time.deltaTime);
                }

                if (Quaternion.Angle(context.Agent.transform.rotation, targetRotation) > 1f) { // Perguntando se o inimigo já está virado para o jogador
                    return State.Running;
                }
            }
        }

        blackboard.IsInAttackRange = true;
        return State.Success;
    }

    public override void OnStop() {
        canRetreat = true;
        isRetreating = false;
        context.Agent.updateRotation = true;
        if (context.Anim != null && context.Anim.enabled) context.Anim.SetBool("IsWalking", false);
    }
}
