using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceToPathAction : ActionNode
{
    [SerializeField] float maxDistance;
    Transform closestLocation;
    protected override State OnUpdate() {
        closestLocation = Waypoints.Instance.GetClosestPoint(context.path, context.agent.transform);

        if (Vector3.Distance(context.agent.transform.position, closestLocation.position) >= maxDistance) blackboard.IsCloseToPath = false;
        else blackboard.IsCloseToPath = true;

        return State.Success;
    }
}
