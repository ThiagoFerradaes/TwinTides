using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/GhostlyWhispers")]
public class GhostlyWhispers : CommonRelic {

    [Header("Area Level 1")]
    public Vector3 Area;
    public float MaxRange;
    public float AreaDuration;
    public float DamageInterval;
    public float Damage;
    public int AmountOfStacks;
    public Material NormalAreaMaterial;
    public EventReference NormalPuddleSound;

    [Header ("Area Level 2")]
    public float DamageLevel2;
    public int AmountOfStacksLevel2;
    public float ObjectDurationLevel2;

    [Header("Area Level 3")]
    public float AreaDurationLevel3;
    public float DamageIntervalLevel3;
    public Material SuperAreaMaterial;
    public EventReference SuperPuddleSound;

    [Header("Area Level 4")]
    public Vector3 AreaLevel4;
    public float AreaDurationLevel4;
    public float DamageIntervalLevel4;
    public int AmountOfStacksLevel4;
    public Material MegaAreaMaterial;
    public float ObjectDurationLevel4;
    public EventReference MegaPuddleSound;
}
