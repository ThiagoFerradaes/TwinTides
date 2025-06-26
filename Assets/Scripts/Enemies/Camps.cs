using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
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
    [SerializeField] bool isBlackBeard;
    [SerializeField, Tooltip("Só necessário quando é random")] int numberOfEnemies;
    [SerializeField, Tooltip("Deixa 0 se n quiser que ele respawne")] float respawnTime;
    [SerializeField] float timeToRestartCamp;

    Chest chest;
    Coroutine restartRoutineCampRoutine;

    HashSet<GameObject> listOfPlayers = new();

    public static event Action OnAllEnemiesDeadStatic;
    public event Action OnAllEnemiesDead;
    public static event Action OnLegendaryCampDefeat;
    public static event Action OnBlackBeardFound;

    bool campIsActive;
    #endregion

    #region Initialize
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
    #endregion

    #region CampSetUp
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
        campIsActive = true;

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

            enemy.SetActive(true);

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

        }
    }

    private void ClearPreviousEvents() {
        foreach (var enemy in currentActiveEnemies) {
            if (!enemy.TryGetComponent<HealthManager>(out var health)) continue;
            if (health != null)
                health.OnDeath -= HandleEnemyDeath;
        }
    }
    #endregion

    #region CampDead
    private void HandleEnemyDeath() {
        aliveCount--;
        if (aliveCount <= 0) {

            OnAllEnemiesDead?.Invoke();
            OnAllEnemiesDeadStatic?.Invoke();
            if (chest != null && chest.rarity == Chest.ChestRarity.Rare) OnLegendaryCampDefeat?.Invoke();

            if(chest != null) chest.UnlockChest();

            if (respawnTime > 0) StartCoroutine(RespawnCampTimer());

            campIsActive = false;

            MusicInGameManager.Instance.SetMusicState(MusicState.Exploration);
        }

    }

    public void KillCamp() {

        Debug.Log("KillCamp chamado");

        foreach (var enemy in currentActiveEnemies) {
            Debug.Log($"Tentando matar: {enemy.name}");

            var health = enemy.GetComponent<HealthManager>();
            if (health == null) {
                Debug.LogWarning($"{enemy.name} não tem HealthManager");
                continue;
            }

            health.Kill();
        }
    }

    #endregion

    #region Respawn
    IEnumerator RespawnCampTimer() {
        yield return new WaitForSeconds(respawnTime);

        RespawnCamp();
    }

    void RespawnCamp() {
        StartCamp(!randomCamp);
    }
    #endregion

    #region CampDetection

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        listOfPlayers.Add(other.gameObject);

        if (campIsActive) {
            if (!isBlackBeard) MusicInGameManager.Instance.SetMusicState(MusicState.Combat);
            else {
                OnBlackBeardFound?.Invoke();
                MusicInGameManager.Instance.SetMusicState(MusicState.Boss);
            }
        }

        foreach (var enemy in currentActiveEnemies) {
            BehaviourTreeRunner behaviour = enemy.GetComponent<BehaviourTreeRunner>();
            behaviour.context.Blackboard.Target = other.transform;
            behaviour.context.Blackboard.TargetInsideCamp = true;
            behaviour.context.Blackboard.IsTargetInRange = true; 
        }

        if (restartRoutineCampRoutine != null) {
            StopCoroutine(restartRoutineCampRoutine);
            restartRoutineCampRoutine = null;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        listOfPlayers.Remove(other.gameObject);

        if (listOfPlayers.Count <= 0) {
            restartRoutineCampRoutine ??= StartCoroutine(CheckIfShouldRestartCamp());
        }
    }

    IEnumerator CheckIfShouldRestartCamp() {
        // Depois, espera o tempo configurado antes de resetar
        yield return new WaitForSeconds(timeToRestartCamp);

        foreach (var enemy in currentActiveEnemies) {
            enemy.GetComponent<HealthManager>().RestartHealth(100);
            var behaviour = enemy.GetComponent<BehaviourTreeRunner>();
            behaviour.context.Blackboard.TargetInsideCamp = false;
        }

        while (AnyEnemyStillInCombat()) {
            yield return new WaitForSeconds(0.5f);
        }

        RestartCamp();
    }
    private bool AnyEnemyStillInCombat() {
        foreach (var enemy in currentActiveEnemies) {
            if (!enemy.activeSelf) continue;

            var bt = enemy.GetComponent<BehaviourTreeRunner>();
            var bb = bt?.context?.Blackboard;

            if (bb == null) continue;

            if (bb.IsCloseToPath && bb.CanFollowPlayer)
                return true;
        }

        return false;
    }

    void RestartCamp() {
        restartRoutineCampRoutine = null;
        if(campIsActive) MusicInGameManager.Instance.SetMusicState(MusicState.Exploration);

        foreach (var enemy in currentActiveEnemies) {
            enemy.GetComponent<HealthManager>().RestartHealth(100);
            var behaviour = enemy.GetComponent<BehaviourTreeRunner>();
            behaviour.context.Blackboard.TargetInsideCamp = false;
            behaviour.context.Blackboard.IsTargetInRange = false;
            behaviour.context.Blackboard.Target = null;
        }
    }



    #endregion

    public int ReturnAliveCount() {
        return aliveCount;
    }
}
