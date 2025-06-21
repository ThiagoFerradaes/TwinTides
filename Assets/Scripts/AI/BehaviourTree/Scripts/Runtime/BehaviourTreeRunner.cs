using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour {

    public BehaviourTree tree;
    HealthManager health;
    public Context context;

    void Awake() {
        tree = tree.Clone();
        context = CreateBehaviourTreeContext();
        tree.Bind(context);
        health = GetComponent<HealthManager>();
    }
    private void Start() {
        PlayersDeathManager.OnGameRestart += RestartBlackBoard;
    }
    private void OnDestroy() {
        PlayersDeathManager.OnGameRestart -= RestartBlackBoard;
    }
    void Update() {
        if (tree && !health.ReturnDeathState()) {
            tree.Update();
        }
    }

    Context CreateBehaviourTreeContext() {
        return Context.CreateFromGameObject(gameObject, tree.blackboard, this);
    }

    public void RestartBlackBoard() {
        GetComponent<HealthManager>().ReviveHandler(100);
        context.Blackboard.Restart();
    }

    public void RestartBlackBoardCamps() {
        context.Blackboard.RestartPaths();
    }

    public void SetPath(Transform originPoint) {
        if (context == null || context.Blackboard == null) {
            Debug.LogWarning("BehaviourTreeRunner context or blackboard is null.");
            return;
        }

        context.Blackboard.OriginPoint(originPoint);

    }
}
