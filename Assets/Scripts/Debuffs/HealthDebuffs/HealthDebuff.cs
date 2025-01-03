using System.Collections;
using UnityEngine;

public abstract class HealthDebuff : Debuff {
    public int InicialStack { get; }
    public int AddStacks { get; }
    public int MaxAmountOfStacks { get; }
    public abstract IEnumerator ApplyDebuff(HealthManager health, int currentStacks);
}
