using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Context {
    public GameObject GameObject;
    public Transform Transform;
    public NavMeshAgent Agent;
    public MovementManager MManager;
    public Blackboard Blackboard;
    public MonoBehaviour CoroutineRunner;
    public Animator Anim;
    public Camps Camp;

    public static Context CreateFromGameObject(GameObject gameObject, Blackboard blackboard, MonoBehaviour coroutineRunner) {
        Context context = new() {
            GameObject = gameObject,
            Transform = gameObject.transform,
            Agent = gameObject.GetComponent<NavMeshAgent>(),
            MManager = gameObject.GetComponent<MovementManager>(),
            Blackboard = blackboard,
            CoroutineRunner = coroutineRunner,
            Anim = gameObject.GetComponentInChildren<Animator>(),
            Camp = gameObject.GetComponentInParent<Camps>()
        };

        return context;
    }

}
