using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : ScriptableObject {

    public List<GameObject> ListOfPrefabs = new();
    public List<string> ListOfPrefabsNames = new();

}
