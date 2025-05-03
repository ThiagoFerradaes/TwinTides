using UnityEngine;

public class FollowPlayerAction : ActionNode {
    [SerializeField] float stoppingDistance;
    protected override State OnUpdate() {
        if (blackboard.Target == null) return State.Failure;

        Vector3 direction = (blackboard.Target.position - context.Agent.transform.position);
        direction.y = 0;

        if (Vector3.Distance(context.Agent.transform.position, blackboard.Target.position) >= stoppingDistance) { // Perguntando se o inimigo já esta perto o suficiente do jogador
            context.Agent.speed = context.MManager.ReturnMoveSpeed();
            context.Agent.SetDestination(blackboard.Target.position);
            return State.Running;
        }

        else {
            context.Agent.speed = 0;
            context.Agent.ResetPath();
            context.Agent.velocity = Vector3.zero; // Esses três parecem ser necessários para parar completamente a movimentação do inimigo

            Quaternion targetRotation = Quaternion.LookRotation(direction); 
            if (direction.sqrMagnitude >= 0.1f) { // forçando o inimigo a virar para o jogador
                context.Agent.transform.rotation = Quaternion.RotateTowards(context.Agent.transform.rotation, targetRotation, context.Agent.angularSpeed * Time.deltaTime);
            }

            if (Quaternion.Angle(context.Agent.transform.rotation, targetRotation) > 1f) { // Perguntando se o inimigo já está virado para o jogador
                return State.Running;
            }
        }
        return State.Success;
    }
}
