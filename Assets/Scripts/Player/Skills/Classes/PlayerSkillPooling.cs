using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkillPooling : NetworkBehaviour {
    public static PlayerSkillPooling Instance;
    Dictionary<string, Queue<GameObject>> objectPooling = new();
    Dictionary<string, Queue<GameObject>> activeSkills = new();

    [HideInInspector] public GameObject MelGameObject, MaevisGameObject;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }
    private void Start() {
        PlayerSetUp.OnPlayerSpawned += PlayerSetUp_OnPlayerSpawned;
    }

    private void PlayerSetUp_OnPlayerSpawned(GameObject obj) {
        if (!obj.TryGetComponent<PlayerSetUp>(out PlayerSetUp player)) return;

        if (player.Character == Characters.Maevis) MaevisGameObject = obj;
        else MelGameObject = obj;
    }

    [Rpc(SendTo.Server)]
    public void InstantiateAndSpawnRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);
        GameObject spawnedObject;

        if (!SkillActive(skill, objectIndex) || skill.IsStackable) {
            spawnedObject = ReturnObjectFroomPooling(skill.skillPrefabs[objectIndex]);

            if (!spawnedObject.GetComponent<NetworkObject>().IsSpawned) {
                spawnedObject.GetComponent<NetworkObject>().Spawn();
            }

            spawnedObject.GetComponent<SkillObjectPrefab>().TurnOnSkillRpc(skillId, skillsLevel, context);
        }
        else {
            spawnedObject = activeSkills[skill.skillPrefabs[objectIndex].name].Dequeue();
            if(!spawnedObject.TryGetComponent<SkillObjectPrefab>(out SkillObjectPrefab obj)) return;
            obj.AddStackRpc();

            string objectName = spawnedObject.name.Replace("(Clone)", "");
            if (activeSkills.ContainsKey(objectName)) {
                activeSkills[objectName].Enqueue(spawnedObject);
            }
        }
    }

    GameObject ReturnObjectFroomPooling(GameObject prefab) {
        if (!objectPooling.ContainsKey(prefab.name)) {
            objectPooling.Add(prefab.name, new Queue<GameObject>());
        }
        if (!activeSkills.ContainsKey(prefab.name)) {
            activeSkills.Add(prefab.name, new Queue<GameObject>());
        }

        GameObject objectToReturn;

        if (objectPooling[prefab.name].Count > 0) {
            objectToReturn = objectPooling[prefab.name].Dequeue();
        }

        else {
            objectToReturn = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        activeSkills[prefab.name].Enqueue(objectToReturn);

        return objectToReturn;
    }

    public void ReturnObjectToPool(GameObject objectToReturn) {
        if (!IsServer) return;

        string objectName = objectToReturn.name.Replace("(Clone)", "");
        if (activeSkills.ContainsKey(objectName)) {
            activeSkills[objectName].Dequeue();
        }
        if (objectPooling.ContainsKey(objectName)) {
            objectPooling[objectName].Enqueue(objectToReturn);
            objectToReturn.GetComponent<SkillObjectPrefab>().TurnOffSkillRpc();
        }
    }

    public bool SkillActive(Skill skill, int objectIndex) {
        if (activeSkills.ContainsKey(skill.skillPrefabs[objectIndex].name)) {
            if (activeSkills[skill.skillPrefabs[objectIndex].name].Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
}
