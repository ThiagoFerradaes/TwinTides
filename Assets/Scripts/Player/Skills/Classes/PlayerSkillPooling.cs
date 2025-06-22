using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class PlayerSkillPooling : NetworkBehaviour {
    public static PlayerSkillPooling Instance;

    // Pool de objetos disponíveis
    private Dictionary<string, Queue<GameObject>> objectPooling = new();

    // Objetos ativos na cena
    private Dictionary<string, List<GameObject>> activeSkills = new();

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
        PlayerSetUp.OnPlayerTwoSpawned += PlayerSetUp_OnPlayerSpawned;
    }

    private void PlayerSetUp_OnPlayerSpawned(GameObject obj) {
        if (!obj.TryGetComponent<PlayerSetUp>(out PlayerSetUp player)) return;

        if (player.Character == Characters.Maevis)
            MaevisGameObject = obj;
        else
            MelGameObject = obj;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestInstantiateRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        InstantiateRpc(skillId, context, skillsLevel, objectIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InstantiateRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {

        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);
        string prefabName = skill.skillPrefabs[objectIndex].name;

        GameObject spawnedObject;

        if (!IsSkillActive(skill, objectIndex) || skill.IsStackable) {
            spawnedObject = GetFromPool(skill.skillPrefabs[objectIndex]);
            spawnedObject.GetComponent<SkillObjectPrefab>().TurnOnSkill(skillId, skillsLevel, context);
            AddToActiveList(prefabName, spawnedObject);
        }
        else {
            var activeList = activeSkills[prefabName];
            if (activeList.Count > 0) {
                spawnedObject = activeList[0];
                if (spawnedObject.TryGetComponent<SkillObjectPrefab>(out var obj)) {
                    obj.AddStack();
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void RequestInstantiateNoChecksRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        InstantiateNoCheckRpc(skillId, context, skillsLevel, objectIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InstantiateNoCheckRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);
        GameObject spawnedObject = GetFromPool(skill.skillPrefabs[objectIndex]);
        AddToActiveList(skill.skillPrefabs[objectIndex].name, spawnedObject);
        spawnedObject.GetComponent<SkillObjectPrefab>().TurnOnSkill(skillId, skillsLevel, context);
    }

    private GameObject GetFromPool(GameObject prefab) {
        string name = prefab.name;

        if (!objectPooling.ContainsKey(name)) {
            objectPooling[name] = new Queue<GameObject>();
        }

        if (!objectPooling[name].TryDequeue(out GameObject pooledObj)) {
            pooledObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        return pooledObj;
    }

    private void AddToActiveList(string objectName, GameObject obj) {
        if (!activeSkills.ContainsKey(objectName)) {
            activeSkills[objectName] = new List<GameObject>();
        }

        if (!activeSkills[objectName].Contains(obj)) {
            activeSkills[objectName].Add(obj);
        }
    }

    public void ReturnObjectToPool(GameObject objectToReturn) {
        string objectName = objectToReturn.name.Replace("(Clone)", "");
        // Remove da lista de ativos
        if (activeSkills.ContainsKey(objectName)) {
            activeSkills[objectName].Remove(objectToReturn);
        }

        // Adiciona de volta ao pool
        if (!objectPooling.ContainsKey(objectName)) {
            objectPooling[objectName] = new Queue<GameObject>();
        }

        objectPooling[objectName].Enqueue(objectToReturn);
        objectToReturn.GetComponent<SkillObjectPrefab>().TurnOffSkill();
    }

    public bool IsSkillActive(Skill skill, int objectIndex) {
        string objectName = skill.skillPrefabs[objectIndex].name;

        if (activeSkills.TryGetValue(objectName, out var list)) {
            return list.Count > 0;
        }

        return false;
    }
}

