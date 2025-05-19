using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Waypoints Instance;
    public enum PathTag {
        LocalA,
        LocalB,
        CenterOfArena,
        LocalC,
        LocalD,
        LocalE,
        LocalF,
    }
    public List<Transform> PathWayPoints = new();
    Dictionary<Transform, PathTag> pathDictionary = new(); 

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        for (int i = 0; i < PathWayPoints.Count; i++) {
            if (i < System.Enum.GetValues(typeof(PathTag)).Length) {
                PathTag tag = (PathTag)i;
                pathDictionary[PathWayPoints[i]] = tag;
            }
            else {
                Debug.Log("Numero de waypoints excedeu o numero de tags definidas");
            }
        }
    }

    public Transform GetClosestPoint(AIPath path, Transform originPoint) {
        Transform closestPoint = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < path.Waypoints.Length; i++) {
            Transform point = GetPointByTag(path.Waypoints[i]);
            if (Vector3.Distance(originPoint.position, point.position) < closestDistance) {
                closestPoint = point;
                closestDistance = Vector3.Distance(originPoint.position, point.position);
            }
        }

        return closestPoint;
    }

    public Transform GetPointByTag(PathTag tag) {
        return pathDictionary.FirstOrDefault(x => x.Value == tag).Key;
    }
}
