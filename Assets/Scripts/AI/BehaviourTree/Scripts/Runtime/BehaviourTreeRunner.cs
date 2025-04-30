using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour {

    public BehaviourTree tree;
    public AIPath path;

    Context context;

    void Start() {
        context = CreateBehaviourTreeContext();
        tree = tree.Clone();
        tree.Bind(context);
    }

    void Update() {
        if (tree) {
            tree.Update();
        }
    }

    Context CreateBehaviourTreeContext() {
        return Context.CreateFromGameObject(gameObject, path, tree.blackboard);
    }

}
