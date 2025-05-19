using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public List<Camps> ListOfCamps = new();
    public List<GameObject> ListOfEnemies = new();

    public static EnemiesManager Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }

    private void Start() {
        foreach (var camp in ListOfCamps) {
            foreach (var enemy in camp.ReturnListOfEnemies()) {
                ListOfEnemies.Add(enemy);
            }
        }
    }
    public int TransformEnemyInId(GameObject enemy) {
        return ListOfEnemies.IndexOf(enemy);
    }

    public GameObject TransformIdInEnemy(int id) {
        return ListOfEnemies[id];
    }
}
