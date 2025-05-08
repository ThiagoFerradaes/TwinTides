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
    public bool IsInAttackRange = false;
    public bool CanAttack = true;

    // Atributos
    public int CurrentPathIndex;
    public int CurrentComboIndex;
    public float AttackRange;
    public float AttackCooldown;
    [HideInInspector] public float GlobalAttackTimer;

    // Cooldowns
    public Dictionary<string, float> Cooldowns = new();

    public enum BlackBoardBools {
        isCloseToPath,
        isTargetInRange,
        canFollowPlayer,
        isAttacking,
        isInAttackRange,
        canAttack
    }

    public bool ReturnBoolByTag(BlackBoardBools tag) {
        return tag switch {
            BlackBoardBools.isCloseToPath => IsCloseToPath,
            BlackBoardBools.isTargetInRange => IsTargetInRange,
            BlackBoardBools.canFollowPlayer => CanFollowPlayer,
            BlackBoardBools.isInAttackRange => IsInAttackRange,
            BlackBoardBools.canAttack => CanAttack,
            BlackBoardBools.isAttacking => IsAttacking,
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
            case BlackBoardBools.isInAttackRange:
                return ref IsInAttackRange;
            case BlackBoardBools.canAttack:
                return ref CanAttack;
            case BlackBoardBools.isAttacking:
                return ref IsAttacking;
            default: return ref IsCloseToPath;
        }
    }

}
