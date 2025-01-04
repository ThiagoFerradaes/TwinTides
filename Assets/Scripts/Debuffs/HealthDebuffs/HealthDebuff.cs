using System.Collections;
using UnityEngine;

public abstract class HealthDebuff : Debuff {
    public int InicialStack;
    public int AddStacks;
    public int MaxAmountOfStacks;
    public abstract IEnumerator ApplyDebuff(HealthManager health, int currentStacks);
}
