using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySkillPooling : NetworkBehaviour
{
    public static EnemySkillPooling Instance;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public Dictionary<string, List<GameObject>> attackDictionary = new();

    [Rpc(SendTo.Server)]
    public void RequestInstantiateAttakcRpc(int skillId, int objectId) {
        InstantiateAttackRpc(skillId, objectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InstantiateAttackRpc(int skillId, int objectId) {

        EnemyAttack attack = EnemySkillConverter.Instance.TransformIdInSkill(skillId);

        GameObject prefab = attack.ListOfPrefabs[objectId];

        GameObject newAttack = GetObjectFromPool(prefab);

        newAttack.GetComponent<EnemyAttackPrefab>().StartAttack();
    }

    GameObject GetObjectFromPool(GameObject prefab) {
        string name = prefab.name;

        for (int i = 0; i < attackDictionary[name].Count; i++) {
            if (!attackDictionary[name][i].activeInHierarchy) return attackDictionary[name][i];
        }

        GameObject newAttackObject = Instantiate(prefab);
        newAttackObject.SetActive(false);
        attackDictionary[name].Add(newAttackObject);

        return newAttackObject;
    }
}
