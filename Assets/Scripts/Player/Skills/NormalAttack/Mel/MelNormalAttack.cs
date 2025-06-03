using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "NormalAttack/Mel")]
public class MelNormalAttack : AttackSkill
{
    [Header("Atributes")]
    public Vector3 SphereSize;
    public float SphereSpeed;
    public float SphereDistance;
    public float SphereDamage;
    public EventReference AttackSound;
}
