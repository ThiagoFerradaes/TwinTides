using UnityEngine;
using UnityEngine.AI;

public class AIContext {
    public AIPath Path;
    public NavMeshAgent Agent;

    public AIContext(AIPath path, NavMeshAgent agent) {
        this.Path = path;
        this.Agent = agent;
    }
}
