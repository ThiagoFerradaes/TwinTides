using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Blackboard {
    public Transform Target;
    public bool IsCloseToPath;
    public bool IsTargetInRange;

    public enum BlackBoardBools {
        isCloseToPath,
        isTargetInRange
    }

    public bool ReturnBoolByTag(BlackBoardBools tag) {
        return tag switch {
            BlackBoardBools.isCloseToPath => IsCloseToPath,
            BlackBoardBools.isTargetInRange => IsTargetInRange,
            _ => false,
        };
    }
}
