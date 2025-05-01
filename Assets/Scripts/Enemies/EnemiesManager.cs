using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public List<GameObject> listOfEnemies;

    public static EnemiesManager Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public int TransformEnemyInId(GameObject enemy) {
        return listOfEnemies.IndexOf(enemy);
    }

    public GameObject TransformIdInEnemy(int id) {
        return listOfEnemies[id];
    }
}
