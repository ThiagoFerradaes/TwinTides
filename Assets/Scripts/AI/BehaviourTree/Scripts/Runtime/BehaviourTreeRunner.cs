using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour {

    public BehaviourTree tree;
    public AIPath path;

    public Context context;

    void Start() {
        tree = tree.Clone();
        context = CreateBehaviourTreeContext();
        tree.Bind(context);
    }

    void Update() {
        if (tree) {
            tree.Update();
        }
    }

    Context CreateBehaviourTreeContext() {
        return Context.CreateFromGameObject(gameObject, path, tree.blackboard, this);
    }

}
