using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkillPooling : NetworkBehaviour {
    public static PlayerSkillPooling Instance;
    Dictionary<string, Queue<GameObject>> objectPooling = new();

    private void Awake() {
        if (Instance == null){
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }

    [Rpc(SendTo.Server)]
    public void InstantiateAndSpawnRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {

        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);

        GameObject spawnedObject = ReturnObjectFroomPooling(skill.skillPrefabs[objectIndex]);

        if (!spawnedObject.GetComponent<NetworkObject>().IsSpawned) {
            spawnedObject.GetComponent<NetworkObject>().Spawn();
        }

        spawnedObject.GetComponent<SkillObjectPrefab>().TurnOnSkillRpc(skillId, skillsLevel, context);
    }

    GameObject ReturnObjectFroomPooling(GameObject prefab) {
        if (!objectPooling.ContainsKey(prefab.name)) {
            objectPooling.Add(prefab.name, new Queue<GameObject>());
        }

        GameObject objectToReturn;

        if (objectPooling[prefab.name].Count > 0) {
            objectToReturn = objectPooling[prefab.name].Dequeue();
        }

        else {
            objectToReturn = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        return objectToReturn;
    }

    public void ReturnObjectToPool(GameObject objectToReturn) {
        if (!IsServer) return;

        string objectName = objectToReturn.name.Replace("(Clone)", "");
        if (objectPooling.ContainsKey(objectName)) {
            objectPooling[objectName].Enqueue(objectToReturn);
            objectToReturn.GetComponent<SkillObjectPrefab>().TurnOffSkillRpc();
        }

    }
}
