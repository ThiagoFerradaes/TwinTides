using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Cross Attack")]
public class BlackBeardCrossAttackSo : BlackBeardSO
{
    [Header("Cut")]
    public float DistanceToCenter;
    public int AmountOfCuts;
    public int AmountOfCutsStronger;
    public float CutSpeed;
    public float CutSpeedStronger;
    public float CutDamage;
    public float CutDamageStronger;
    public Vector3 CutSize;
    public float CutRange;
    public EventReference CutSound;

    [Header("Field")]
    public float FieldDuration;
    public float FieldDamage;
    public float TimeBetweenDamage;
    public Vector2 FieldSize;
    public EventReference FieldSound;

    [Header("Animation")]
    public string AnimationName;
    public string AnimationName2;
    public string AnimationTriggerName;
    public string AnimationTriggerName2;
    [Range(0, 1)] public float AnimationPercentToAttack;
}
