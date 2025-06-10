using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour {

    public BehaviourTree tree;
    public AIPath path;

    public Context context;

    [SerializeField] bool isPatrol;

    void Awake() {
        tree = tree.Clone();
        context = CreateBehaviourTreeContext();
        tree.Bind(context);
    }
    private void Start() {
        PlayersDeathManager.OnGameRestart += RestartBlackBoard;
    }
    private void OnDestroy() {
        PlayersDeathManager.OnGameRestart -= RestartBlackBoard;
    }
    void Update() {
        if (tree) {
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

        List<Transform> listOfPoints = new();

        if (!isPatrol) { listOfPoints.Add(originPoint); }
        else {
            foreach (Waypoints.PathTag tag in path.Waypoints) {
                Transform targetPosition = Waypoints.Instance.GetPointByTag(tag);
                listOfPoints.Add(targetPosition);
            }
        }

        context.Blackboard.SetPath(listOfPoints);
        
    }
}
