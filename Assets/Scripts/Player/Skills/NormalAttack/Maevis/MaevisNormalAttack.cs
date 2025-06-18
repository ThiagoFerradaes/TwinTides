using FMODUnity;
using UnityEngine;

[CreateAssetMenu (menuName = "NormalAttack/Maevis")]
public class MaevisNormalAttack : AttackSkill
{
    [Header("Atibutes")]
    public float DurationOfFirstAttack;
    public float DurationOfSecondAttack;
    public float DurationOfThirdAtack;
    //public float CooldownBetweenEachAttack;

    public float AttackPosition;
    public float ThirdAttackPosition;
    public Vector3 FirstAndSecondAttackSize;
    public Vector3 ThirdAttackSize;

    public float FirstAttackDamage;
    public float SecondAttackDamage;
    public float ThirdAttackDamage;
    public float TimeLimitBetweenEachAttack;

    public EventReference attackSound;

    [Header("Animation")]
    public string AnimationParameterName;
    public string AnimationName;
    public string AnimationName2;
    public string AnimationName3;
    [Range(0,1)]public float AnimationPercentToAttack;

}
