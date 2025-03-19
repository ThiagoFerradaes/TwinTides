using UnityEngine;
using UnityEngine.AI;

public class AIContext {
    public AIPath Path;
    public NavMeshAgent Agent;
    public MovementManager MManager;

    public AIContext(AIPath path, NavMeshAgent agent, MovementManager mManager) {
        this.Path = path;
        this.Agent = agent;
        MManager = mManager;
    }
}
