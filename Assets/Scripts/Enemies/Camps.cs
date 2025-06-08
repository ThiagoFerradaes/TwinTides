using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Camps : NetworkBehaviour {
    #region Variables

    // Listas
    List<GameObject> listOfEnemies = new();
    List<GameObject> currentActiveEnemies = new();
    List<Transform> listOfPoints = new();

    // int
    int aliveCount;

    [SerializeField] bool activateOnStart;
    [SerializeField] bool randomCamp;
    [SerializeField, Tooltip("Só necessário quando é random")] int numberOfEnemies;
    [SerializeField, Tooltip("Deixa 0 se n quiser que ele respawne")] float respawnTime;
    Chest chest;

    public event Action OnAllEnemiesDead;

    #endregion

    void Awake() {
        listOfEnemies.Clear();

        for (int i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i).gameObject;

            if (child.CompareTag("Enemy")) {
                listOfEnemies.Add(child);
            }
            else if (child.CompareTag("CampPoint")) {
                listOfPoints.Add(child.transform);
            }
            else if (child.CompareTag("Chest")) {
                chest = child.GetComponent<Chest>();
            }
        }
    }

    public List<GameObject> ReturnListOfEnemies() {
        return listOfEnemies;
    }

    private void Start() {
        if (activateOnStart) StartCamp(!randomCamp);
    }

    void TurnChestOn() {
        if (chest == null) return;

        chest.gameObject.SetActive(true);
    }

    void StartCamp(bool all) {
        if (all) {
            int[] listWithEveryEnemy = new int[listOfEnemies.Count];
            for (int i = 0; i < listOfEnemies.Count; i++) {
                listWithEveryEnemy[i] = i;
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

            StartCampWithIndex(randomIndexes.ToArray());
        }
        TurnChestOn();
    }

    public void StartCampWithIndex(int[] index) {
        if (!IsServer) return;

        List<int> availablePointIndexes = new();
        for (int i = 0; i < listOfPoints.Count; i++) {
            availablePointIndexes.Add(i);
        }

        for (int i = 0; i < availablePointIndexes.Count; i++) {
            int rand = Random.Range(i, availablePointIndexes.Count);
            (availablePointIndexes[i], availablePointIndexes[rand]) =
                (availablePointIndexes[rand], availablePointIndexes[i]);
        }

        int[] pointIndexesToUse = availablePointIndexes.Take(index.Length).ToArray();

        StartCampForEveryoneRpc(index, pointIndexesToUse);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartCampForEveryoneRpc(int[] index, int[] pointIndexes) {
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

            enemy.transform.position = listOfPoints[pointIndexes[i]].position;

            Vector3 directionToCenter = transform.position - enemy.transform.position;
            directionToCenter.y = 0;
            if (directionToCenter != Vector3.zero) {
                enemy.transform.rotation = Quaternion.LookRotation(directionToCenter);
            }

            BehaviourTreeRunner behaviour = enemy.GetComponent<BehaviourTreeRunner>();
            behaviour.RestartBlackBoard();
            behaviour.RestartBlackBoardCamps();
            behaviour.SetPath(listOfPoints[pointIndexes[i]]);

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
            chest.UnlockChest();
            if (respawnTime > 0) StartCoroutine(RespawnCampTimer());
        }

    }

    #region Respawn
    IEnumerator RespawnCampTimer() {
        yield return new WaitForSeconds(respawnTime);

        RespawnCamp();
    }

    void RespawnCamp() {
        StartCamp(!randomCamp);
    }
    #endregion

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
