using UnityEngine;
using UnityEngine.AI;

public class AIContext {
    public AIPath Path;
    public NavMeshAgent Agent;
    public MovementManager MManager;
    public BlackBoard Blackboard;

    public AIContext(AIPath path, NavMeshAgent agent, MovementManager mManager, BlackBoard blackboard) {
        this.Path = path;
        this.Agent = agent;
        MManager = mManager;
        Blackboard = blackboard;
    }
}
