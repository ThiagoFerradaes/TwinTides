using UnityEngine;

[CreateAssetMenu (menuName = "NormalAttack/Maevis")]
public class MaevisNormalAttack : AttackSkill
{
    [Header("Atibutes")]
    public float DurationOfFirstAttack;
    public float DurationOfSecondAttack;
    public float DurationOfThirdAtack;
    public float TimeBetweenEachAttack;

    public Vector3 AttackPosition;
    public Vector3 ThirdAttackPosition;
    public Vector3 AttackRotation;
    public Vector3 ThierdAttackRotation;

    public float FirstAttackDamage;
    public float SecondAttackDamage;
    public float ThirdAttackDamage;
    public float WindowBetweenEachAttack;

}
