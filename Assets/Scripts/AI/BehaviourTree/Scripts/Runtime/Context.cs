using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Context {
    public GameObject gameObject;
    public Transform transform;
    public NavMeshAgent agent;
    public AIPath path;

        public static Context CreateFromGameObject(GameObject gameObject, AIPath path) {
        Context context = new() {
            gameObject = gameObject,
            transform = gameObject.transform,
            agent = gameObject.GetComponent<NavMeshAgent>(),
            path = path
        };

        return context;
    }
}
