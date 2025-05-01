using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[System.Serializable]
public class Blackboard {

    // Componentes
    public Transform Target;

    // Condicionais
    public bool IsCloseToPath = true;
    public bool IsTargetInRange = false;
    public bool CanFollowPlayer = true;
    public bool IsAttacking = false;

    // Atributos
    public int CurrentPathIndex;
    public int CurrentComboIndex;

    public enum BlackBoardBools {
        isCloseToPath,
        isTargetInRange,
        canFollowPlayer,
    }

    public bool ReturnBoolByTag(BlackBoardBools tag) {
        return tag switch {
            BlackBoardBools.isCloseToPath => IsCloseToPath,
            BlackBoardBools.isTargetInRange => IsTargetInRange,
            BlackBoardBools.canFollowPlayer => CanFollowPlayer,
            _ => false,
        };
    }

    public ref bool ReturnRefByTag(BlackBoardBools tag) {
        switch (tag) {
            case BlackBoardBools.isCloseToPath:
                return ref IsCloseToPath;
            case BlackBoardBools.isTargetInRange:
                return ref IsTargetInRange;
            case BlackBoardBools.canFollowPlayer:
                return ref CanFollowPlayer;
            default: return ref IsCloseToPath;
        }
    }

}
