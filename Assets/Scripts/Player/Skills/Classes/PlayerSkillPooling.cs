using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
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
        PlayerSetUp.OnPlayerTwoSpawned += PlayerSetUp_OnPlayerSpawned;
    }

    private void PlayerSetUp_OnPlayerSpawned(GameObject obj) {
        if (!obj.TryGetComponent<PlayerSetUp>(out PlayerSetUp player)) return;

        if (player.Character == Characters.Maevis) MaevisGameObject = obj;
        else MelGameObject = obj;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestInstantiateRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {

        InstantiateRpc(skillId, context, skillsLevel, objectIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InstantiateRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);
        GameObject spawnedObject;

        if (!SkillActive(skill, objectIndex) || skill.IsStackable) { // verificando se o objeto em questão já está ativo na cena
            spawnedObject = ReturnObjectFroomPooling(skill.skillPrefabs[objectIndex]); // Puxando o pooling
            spawnedObject.GetComponent<SkillObjectPrefab>().TurnOnSkill(skillId, skillsLevel, context); // ligando o objeto 
        }
        else { // o objeto em questão já está ativo
            spawnedObject = activeSkills[skill.skillPrefabs[objectIndex].name].Dequeue(); // achando o objeto ativo

            if (!spawnedObject.TryGetComponent<SkillObjectPrefab>(out SkillObjectPrefab obj)) return;
            obj.AddStackRpc(); // chamando a função de stacks

            string objectName = spawnedObject.name.Replace("(Clone)", "");
            if (activeSkills.ContainsKey(objectName)) {
                activeSkills[objectName].Enqueue(spawnedObject);
            }
        }
    }


    [Rpc(SendTo.Server)]
    public void RequestInstantiateNoChecksRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        InstantiateNoCheckRpc(skillId, context, skillsLevel, objectIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void InstantiateNoCheckRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {
        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);
        GameObject spawnedObject;

        spawnedObject = ReturnObjectFroomPooling(skill.skillPrefabs[objectIndex]); // Puxando o pooling
        spawnedObject.GetComponent<SkillObjectPrefab>().TurnOnSkill(skillId, skillsLevel, context); // ligando o objeto 
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

        string objectName = objectToReturn.name.Replace("(Clone)", "");

        if (activeSkills.ContainsKey(objectName)) {
            activeSkills[objectName].Dequeue();
        }

        if (objectPooling.ContainsKey(objectName)) {
            objectPooling[objectName].Enqueue(objectToReturn);
            objectToReturn.GetComponent<SkillObjectPrefab>().TurnOffSkill();
        }
    }

    public bool SkillActive(Skill skill, int objectIndex) {
        if (activeSkills.TryGetValue(skill.skillPrefabs[objectIndex].name, out var queue)) {
            return queue.Count > 0;
        }
        return false;
    }
}
