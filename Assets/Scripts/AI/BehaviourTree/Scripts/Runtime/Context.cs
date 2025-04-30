using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Context {
    public GameObject GameObject;
    public Transform Transform;
    public NavMeshAgent Agent;
    public AIPath Path;
    public MovementManager MManager;
    public Blackboard Blackboard;

        public static Context CreateFromGameObject(GameObject gameObject, AIPath path, Blackboard blackboard) {
        Context context = new() {
            GameObject = gameObject,
            Transform = gameObject.transform,
            Agent = gameObject.GetComponent<NavMeshAgent>(),
            Path = path,
            MManager = gameObject.GetComponent<MovementManager>(),
            Blackboard = blackboard,
        };

        return context;
    }
}
