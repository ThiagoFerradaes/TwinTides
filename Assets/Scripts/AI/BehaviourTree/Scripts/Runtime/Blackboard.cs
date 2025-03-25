using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[System.Serializable]
public class Blackboard {
    public Transform Target;
    public bool IsCloseToPath;
    public bool IsTargetInRange;
    public bool CanDetectPlayer = true;

    public enum BlackBoardBools {
        isCloseToPath,
        isTargetInRange,
        canDetectPlayer,
    }

    public bool ReturnBoolByTag(BlackBoardBools tag) {
        return tag switch {
            BlackBoardBools.isCloseToPath => IsCloseToPath,
            BlackBoardBools.isTargetInRange => IsTargetInRange,
            BlackBoardBools.canDetectPlayer => CanDetectPlayer,
            _ => false,
        };
    }

    //public void ReturnVariableByTag(BlackBoardBools tag, bool value) {
    //    switch (tag) {
    //        case BlackBoardBools.isCloseToPath:
    //            IsCloseToPath = value; break;
    //        case BlackBoardBools.isTargetInRange:
    //            IsTargetInRange = value; break;
    //        case BlackBoardBools.canDetectPlayer:
    //            CanDetectPlayer = value; break;
    //    }
    //}

    public ref bool ReturnRefByTag(BlackBoardBools tag) {
        switch (tag) {
            case BlackBoardBools.isCloseToPath:
                return ref IsCloseToPath;
            case BlackBoardBools.isTargetInRange:
                return ref IsTargetInRange;
            case BlackBoardBools.canDetectPlayer:
                return ref CanDetectPlayer;
            default: return ref IsCloseToPath;
        }
    }

}
