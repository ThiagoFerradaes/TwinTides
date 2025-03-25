using UnityEngine;

[CreateAssetMenu(fileName = "AIPath", menuName = "BehaviourTree/AIPath")]
public class AIPath : ScriptableObject
{
    public Waypoints.PathTag[] Waypoints;

}
