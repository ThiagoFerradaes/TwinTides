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

    public static event Action OnAllEnemiesDeadStatic;
    public event Action OnAllEnemiesDead;
    public static event Action OnLegendaryCampDefeat;

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
        if (all) { // Campo não é aleatorio, spawna todos os inimigos
            int[] listWithEveryEnemy = new int[listOfEnemies.Count];
            for (int i = 0; i < listOfEnemies.Count; i++) {
                listWithEveryEnemy[i] = i;
            }

            StartCampWithIndex(listWithEveryEnemy);
        }
        else { // Campo é aleatório, spawna uma quantia dos inimigos
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

        for (int i = 0; i < index.Length; i++) {
            int enemyIndex = index[i];
            if (i >= pointIndexes.Length) {
                Debug.LogWarning($"pointIndexes não tem índice {i}. Ignorando este inimigo.");
                continue;
            }

            int pointIndex = pointIndexes[i];

            if (enemyIndex < 0 || enemyIndex >= listOfEnemies.Count) {
                Debug.LogWarning($"enemyIndex {enemyIndex} fora dos limites.");
                continue;
            }

            if (pointIndex < 0 || pointIndex >= listOfPoints.Count) {
                Debug.LogWarning($"pointIndex {pointIndex} fora dos limites.");
                continue;
            }

            var enemy = listOfEnemies[enemyIndex];
            if (!enemy.TryGetComponent<HealthManager>(out var health)) continue;

            health.OnDeath += HandleEnemyDeath;
            currentActiveEnemies.Add(enemy);
            aliveCount++;

            enemy.transform.position = listOfPoints[pointIndex].position;

            Vector3 directionToCenter = transform.position - enemy.transform.position;
            directionToCenter.y = 0;
            if (directionToCenter != Vector3.zero) {
                enemy.transform.rotation = Quaternion.LookRotation(directionToCenter);
            }

            BehaviourTreeRunner behaviour = enemy.GetComponent<BehaviourTreeRunner>();
            behaviour.RestartBlackBoard();
            behaviour.RestartBlackBoardCamps();
            behaviour.SetPath(listOfPoints[pointIndex]);

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
            OnAllEnemiesDeadStatic?.Invoke();
            if (chest.rarity == Chest.ChestRarity.Rare) OnLegendaryCampDefeat?.Invoke();
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
