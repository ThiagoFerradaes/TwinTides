using System.Collections;
using UnityEngine;

public abstract class HealthBuff : Buff
{
    public int InicialStack;
    public int AddStacks;
    public int MaxAmountOfStacks;
    public float duration;
    public abstract IEnumerator ApplyBuff(HealthManager health, int currentStacks);
    public abstract void StopBuff(HealthManager health);
}
