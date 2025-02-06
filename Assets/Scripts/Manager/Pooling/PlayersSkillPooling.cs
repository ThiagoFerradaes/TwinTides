using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersSkillPooling : NetworkBehaviour {
    public static PlayersSkillPooling Instance;

    readonly Dictionary<string, Queue<GameObject>> poolingDictioinary = new();

    // Mel
    [SerializeField] List<Skill> melCommonSklilIndex = new();
    [SerializeField] List<Skill> melLegendarySkillIndex = new();
    [SerializeField] List<Skill> melAtackSkillIndex = new();

    // Maevis
    [SerializeField] List<Skill> maevisCommonSkillIndex = new();
    [SerializeField] List<Skill> maevisLegendarySkillIndex = new();
    [SerializeField] List<Skill> maevisAtackSkillIndex = new();

    List<Skill> skillIndex = new();

    private void Awake() {
        Instance = this;
        AddSKillsToList();
    }
    void AddSKillsToList() {
        foreach (var skill in melCommonSklilIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in melLegendarySkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in melAtackSkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in maevisCommonSkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in maevisLegendarySkillIndex) {
            skillIndex.Add(skill);
        }
        foreach (var skill in maevisAtackSkillIndex) {
            skillIndex.Add(skill);
        }
    }
    public int TransformSkillInInt(Skill skill) {
        return skillIndex.IndexOf(skill);
    }

    public Skill TransformIdInSkill(int skillId) {
        return skillIndex[skillId];
    }

    [Rpc(SendTo.Server)]
    public void InstanciateObjectRpc(int skillId, SkillContext context, int skillsLevel, int objectIndex) {

        Debug.Log("InstanciateObjectRpc");

        Skill skill = TransformIdInSkill(skillId); // convertemos o id em skill

        GameObject prefab = skill.skillPrefabs[objectIndex]; // pegamos o prefab registrado na skill


        if (!poolingDictioinary.ContainsKey(prefab.name)) { // verificamos se ja existe uma Queue registrada com esse objeto
            poolingDictioinary[prefab.name] = new Queue<GameObject>(); // se n tiver, criamos uma
        }

        GameObject obj = GetObjectFromPool(prefab); // pegamos o objeto do pooling


        if (!obj.GetComponent<NetworkObject>().IsSpawned) { // verificamos se esse objeto ja existe na rede

            Debug.Log("Trying to spawn" + obj.name);

            obj.GetComponent<NetworkObject>().Spawn(true); // se n existia agora existe

            Debug.Log("Spawned");
        }

        if (!obj.TryGetComponent<SkillObjectPrefab>(out SkillObjectPrefab skillObjectPrefab)) { Debug.Log("n achei o script"); }
        else {
            skillObjectPrefab.Test(skillId, skillsLevel, context);
        }

        //obj.GetComponent<SkillObjectPrefab>().TurnOnSkillRpc(skillId, skillsLevel, context); // Chamamos a função para todos
    }

    GameObject GetObjectFromPool(GameObject prefab) {

        GameObject obj;

        if (poolingDictioinary[prefab.name].Count > 0) { // Se já tem algum objeto na fila de forma disponível
            obj = poolingDictioinary[prefab.name].Dequeue(); // obj se torna o objeto da fila
        }
        else {
            obj = Instantiate(prefab);
            poolingDictioinary[prefab.name].Enqueue(obj);
        }
        return obj;
    }

    public void ReturnObjetToQueue(GameObject objectToReturn) {

        poolingDictioinary[objectToReturn.name].Enqueue(objectToReturn);
        objectToReturn.GetComponent<SkillObjectPrefab>().TurnOffSkillRpc();
    }
}
