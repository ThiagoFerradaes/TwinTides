using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Camps : MonoBehaviour
{
    #region Variables

    // Listas
    List<GameObject> listOfEnemies = new();
    List<GameObject> currentActiveEnemies = new();

    // int
    int aliveCount;

    [SerializeField] bool activateOnStart;
    [SerializeField] bool randomCamp;
    [SerializeField] int numberOfEnemies;

    public event Action OnAllEnemiesDead;

    #endregion

    void Awake() {
        listOfEnemies.Clear();

        for (int i = 0; i < transform.childCount; i++) {
            var enemy = transform.GetChild(i).gameObject;
            listOfEnemies.Add(enemy);
        }
    }

    public List<GameObject> ReturnListOfEnemies() {
        return listOfEnemies;
    }

    private void Start() {
        if (activateOnStart) StartCamp(!randomCamp);
    }

    void StartCamp(bool all) {
        if (all) {
            List<int> listWithEveryEnemy = new();
            for (int i = 0; i< listOfEnemies.Count; i++) {
                listWithEveryEnemy.Add(i);
            }

            StartCampWithIndex(listWithEveryEnemy);
        }
        else {
            List<int> randomIndexes = new();

            List<int> allIndexes = new();
            for (int i = 0; i < listOfEnemies.Count; i++) {
                allIndexes.Add(i);
            }

            for (int i = 0; i < numberOfEnemies; i++) {
                int random = Random.Range(0, allIndexes.Count);
                randomIndexes.Add(allIndexes[random]);
                allIndexes.RemoveAt(random); 
            }

            StartCampWithIndex(randomIndexes);
        }
        
    }

    public  void StartCampWithIndex(List<int> index) {
        ClearPreviousEvents();
        currentActiveEnemies = new List<GameObject>();
        aliveCount = 0;

        foreach (int i in index) {
            if (i < 0 || i >= listOfEnemies.Count) continue;

            var enemy = listOfEnemies[i];
            if (!enemy.TryGetComponent<HealthManager>(out var health)) continue;

            health.OnDeath += HandleEnemyDeath;
            currentActiveEnemies.Add(enemy);
            aliveCount++;

            health.ReviveHandler(100);

            enemy.transform.position = this.transform.position;
            enemy.SetActive(true);
        }
    }
    private void ClearPreviousEvents() {
        foreach (var enemy in currentActiveEnemies) {
            if (!enemy.TryGetComponent<HealthManager>(out var health)) continue;
            if (health != null)
                health.OnDeath -= HandleEnemyDeath;
        }
    }
    private void HandleEnemyDeath() {
        aliveCount--;
        if (aliveCount <= 0) {
            OnAllEnemiesDead?.Invoke();
        }
    }

    public void KillCamp() {
        foreach (var enemy in currentActiveEnemies) {
            var health = enemy.GetComponent<HealthManager>();
            health?.Kill();
        }
    }

    public int ReturnAliveCount() {
        return aliveCount;
    }
}
