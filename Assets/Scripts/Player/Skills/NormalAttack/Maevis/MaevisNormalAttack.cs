using UnityEngine;

[CreateAssetMenu (menuName = "NormalAttack/Maevis")]
public class MaevisNormalAttack : AttackSkill
{
    [Header("Atibutes")]
    public float DurationOfFirstAttack;
    public float DurationOfSecondAttack;
    public float DurationOfThirdAtack;
    public float CooldownBetweenEachAttack;

    public Vector3 AttackPosition;
    public Vector3 ThirdAttackPosition;
    public Vector3 AttackRotation;
    public Vector3 ThierdAttackRotation;
    public Vector3 FirstAndSecondAttackSize;
    public Vector3 ThirdAttackSize;

    public float FirstAttackDamage;
    public float SecondAttackDamage;
    public float ThirdAttackDamage;
    public float TimeLimitBetweenEachAttack;

}
