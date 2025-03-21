using UnityEngine;

public class BlackBoard : MonoBehaviour {
    public Transform Target;
    public bool isCloseToPath;
    public bool isTargetInRange;

    public enum BlackBoardBools {
        isCloseToPath,
        isTargetInRange
    }

    public bool ReturnBoolByTag(BlackBoardBools tag) {
        return tag switch {
            BlackBoardBools.isCloseToPath => isCloseToPath,
            BlackBoardBools.isTargetInRange => isTargetInRange,
            _ => false,
        };
    }
}
