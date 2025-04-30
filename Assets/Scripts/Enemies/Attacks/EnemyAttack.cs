using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : ScriptableObject {

    public List<GameObject> ListOfPrefabs = new();

    public enum AttackState { RUNNING, SUCCESS }

    protected AttackState state = AttackState.RUNNING;

    public AttackState State => state;

    public abstract void StartAttack(Context parent);
}
