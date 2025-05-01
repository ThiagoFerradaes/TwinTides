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

    public void RequestInstantiateAttack(EnemyAttack attack, int objectId, GameObject enemy) {
        if (!IsServer) return;

        int skillId = EnemySkillConverter.Instance.TransformSkillInInt(attack);

        int enemyId = EnemiesManager.Instance.TransformEnemyInId(enemy);

        InstantiateAttackRpc(skillId, objectId, enemyId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InstantiateAttackRpc(int skillId, int objectId, int enemyId) {

        EnemyAttack attack = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        GameObject prefab = attack.ListOfPrefabs[objectId];

        GameObject newAttack = GetObjectFromPool(attack, objectId, prefab);

        newAttack.GetComponent<EnemyAttackPrefab>().StartAttack(enemyId, skillId);
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
