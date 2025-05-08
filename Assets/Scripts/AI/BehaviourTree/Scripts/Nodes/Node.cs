using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject {
    public enum State {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public Context context;
    [HideInInspector] public Blackboard blackboard;

    public State Update() {

        if (!started) {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if (state != State.Running) {
            OnStop();
            started = false;
        }

        return state;
    }

    public virtual Node Clone() {
        return Instantiate(this);
    }

    public void Abort() {
        Debug.Log("Abort " + name);
        BehaviourTree.Traverse(this, (node) => {
            node.started = false;
            node.state = State.Running;
            node.OnStop();
        });
    }

    public virtual void OnStart() { }
    public virtual void OnStop() { }
    protected abstract State OnUpdate();
}
