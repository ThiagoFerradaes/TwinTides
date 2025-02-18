using UnityEngine;

[CreateAssetMenu (menuName = "MaevisAttack/NormalAttack")]
public class MaevisNormalAttack : AttackSkill
{
    [Header("Atibutes")]
    public float DurationOfFirstAttack;
    public float DurationOfSecondAttack;
    public float DurationOfThirdAtack;
    public float TimeBetweenEachAttack;

}
