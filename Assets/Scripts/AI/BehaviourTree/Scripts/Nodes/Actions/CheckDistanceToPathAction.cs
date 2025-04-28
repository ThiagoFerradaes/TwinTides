using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceToPathAction : ActionNode
{
    [SerializeField] float maxDistance;
    Transform closestLocation;
    protected override State OnUpdate() {
        closestLocation = Waypoints.Instance.GetClosestPoint(context.Path, context.Agent.transform);

        if (Vector3.Distance(context.Agent.transform.position, closestLocation.position) >= maxDistance) blackboard.IsCloseToPath = false;
        else blackboard.IsCloseToPath = true;

        return State.Success;
    }
}
