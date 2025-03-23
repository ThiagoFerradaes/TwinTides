using UnityEngine;
using UnityEngine.AI;

public class AIContext {
    public AIPath Path;
    public NavMeshAgent Agent;
    public MovementManager MManager;
    public BlackBoard Blackboard;

    public static AIContext CreateContext(AIPath path, GameObject parent) {
        AIContext newContext = new() {
            Path = path,
            Agent = parent.GetComponent<NavMeshAgent>(),
            MManager = parent.GetComponent<MovementManager>(),
            Blackboard = parent.GetComponent<BlackBoard>(),
        };

        return newContext;
    }
}
