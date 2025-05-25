using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceToPathAction : ActionNode
{
    [SerializeField] float maxDistance;
    Transform closestLocation;
    protected override State OnUpdate() {
        if (blackboard.path == null || blackboard.path.Count == 0) {
            blackboard.IsCloseToPath = true; 
            return State.Success;
        }

        closestLocation = GetClosestPoint(blackboard.path, context.Agent.transform.position);

        blackboard.IsCloseToPath = Vector3.Distance(context.Agent.transform.position, closestLocation.position) < maxDistance;

        return State.Success;
    }

    private Transform GetClosestPoint(List<Transform> points, Vector3 position) {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform point in points) {
            float dist = Vector3.Distance(position, point.position);
            if (dist < minDistance) {
                minDistance = dist;
                closest = point;
            }
        }

        return closest;
    }
}
