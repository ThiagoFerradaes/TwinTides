using UnityEngine;

[CreateAssetMenu(menuName = "NormalAttack/Mel")]
public class MelNormalAttack : AttackSkill
{
    [Header("Atributes")]
    public Vector3 SphereSize;
    public float SphereSpeed;
    public float SphereDuration;
    public float SphereDamage;
}
