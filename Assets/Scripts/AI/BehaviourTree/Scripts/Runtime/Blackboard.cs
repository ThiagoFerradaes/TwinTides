using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[System.Serializable]
public class Blackboard {

    // Componentes
    public Transform Target;

    // Listas
    public Transform originPoint;

    // Condicionais
    public bool IsCloseToPath = true;
    public bool IsTargetInRange = false;
    public bool CanFollowPlayer = true;
    public bool IsAttacking = false;
    public bool IsInAttackRange = false;
    public bool CanAttack = true;
    public bool TargetInsideCamp = false;

    // Atributos
    public int CurrentPathIndex;
    public int CurrentComboIndex;
    public float AttackRange;
    public float AttackCooldown;
    public float maxDistanceToPath;
    [HideInInspector] public float GlobalAttackTimer;

    // Cooldowns
    public Dictionary<string, float> Cooldowns = new();

    public enum BlackBoardBools {
        isCloseToPath,
        isTargetInRange,
        canFollowPlayer,
        isAttacking,
        isInAttackRange,
        canAttack,
        targetInsideCamp
    }

    public bool ReturnBoolByTag(BlackBoardBools tag) {
        return tag switch {
            BlackBoardBools.isCloseToPath => IsCloseToPath,
            BlackBoardBools.isTargetInRange => IsTargetInRange,
            BlackBoardBools.canFollowPlayer => CanFollowPlayer,
            BlackBoardBools.isInAttackRange => IsInAttackRange,
            BlackBoardBools.canAttack => CanAttack,
            BlackBoardBools.isAttacking => IsAttacking,
            BlackBoardBools.targetInsideCamp => TargetInsideCamp,
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
            case BlackBoardBools.targetInsideCamp:
                return ref TargetInsideCamp;
            default: return ref IsCloseToPath;
        }
    }

    public void OriginPoint(Transform origin) {
        this.originPoint = origin;
    }

    public void Restart() {
        // Resetar componentes
        Target = null;

        // Resetar condicionais
        IsCloseToPath = true;      // ou false, dependendo do seu default
        IsTargetInRange = false;
        CanFollowPlayer = true;
        IsAttacking = false;
        IsInAttackRange = false;
        CanAttack = true;
        TargetInsideCamp = false;

        // Resetar atributos
        CurrentPathIndex = 0;
        CurrentComboIndex = 0;
        GlobalAttackTimer = 0f;

        // Resetar cooldowns
        Cooldowns.Clear();
    }

    public void RestartPaths() {
        // Resetar listas
        originPoint = null;
    }
}
