using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySkillPooling : NetworkBehaviour
{
    public static EnemySkillPooling Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Dictionary<string, List<GameObject>> attackDictionary = new();

    public void RequestInstantiateAttack(EnemyAttack attack, int objectId, GameObject enemy, Vector3? position = null, float? number = null) {
        if (!IsServer) return;

        int skillId = EnemySkillConverter.Instance.TransformSkillInInt(attack);

        int enemyId = EnemiesManager.Instance.TransformEnemyInId(enemy);

        Vector3 pos = position ?? Vector3.negativeInfinity;
        float num = number ?? float.MaxValue;

        InstantiateAttackRpc(skillId, objectId, enemyId, pos, num);

    }

    [Rpc(SendTo.ClientsAndHost)]
    void InstantiateAttackRpc(int skillId, int objectId, int enemyId, Vector3 position, float number) {

        EnemyAttack attack = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        GameObject prefab = attack.ListOfPrefabs[objectId];

        GameObject newAttack = GetObjectFromPool(attack, objectId, prefab);

        if (!position.Equals(Vector3.negativeInfinity) && !number.Equals(float.MaxValue)) {
            newAttack.GetComponent<EnemyAttackPrefab>().StartAttack(enemyId, skillId, position, number);
        }
        else if (!position.Equals(Vector3.negativeInfinity)) {
            newAttack.GetComponent<EnemyAttackPrefab>().StartAttack(enemyId, skillId, position);
        }
        else {
            newAttack.GetComponent<EnemyAttackPrefab>().StartAttack(enemyId, skillId);
        }
    }

    GameObject GetObjectFromPool(EnemyAttack skill, int objectId, GameObject prefab) {
        string name = skill.ListOfPrefabsNames[objectId];

        if (!attackDictionary.ContainsKey(name)) {
            attackDictionary[name] = new List<GameObject>();
        }

        for (int i = 0; i < attackDictionary[name].Count; i++) {
            if (!attackDictionary[name][i].activeInHierarchy) return attackDictionary[name][i];
        }

        GameObject newAttackObject = Instantiate(prefab);
        newAttackObject.SetActive(false);

        attackDictionary[name].Add(newAttackObject);

        return newAttackObject;
    }
}
