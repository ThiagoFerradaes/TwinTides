using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersSkillPooling : NetworkBehaviour
{
    public static PlayersSkillPooling Instance;

    readonly Dictionary<GameObject, Queue<GameObject>> poolingDictioinary = new();

    private void Awake() {
        Instance = this;
    }

    public GameObject GetObjectFromQueue(GameObject preFab) {
        if (!IsServer) return null;

        Debug.Log("GetObjectFromQueue started");

        if (!poolingDictioinary.ContainsKey(preFab)) { // caso não tenha o objeto no dicionario
            poolingDictioinary[preFab] = new Queue<GameObject>(); // cria uma nova fila de objetos
        }

        // Aqui já tem uma fila de objetos
        GameObject obj;

        if (poolingDictioinary[preFab].Count > 0) { // Se já tem algum objeto na fila
            obj = poolingDictioinary[preFab].Dequeue(); // obj se torna o objeto da fila
        }
        else { // se não tem nenhum objeto disponivel/ na fila
            obj = Instantiate(preFab);

            //if (obj.TryGetComponent<NetworkObject>(out NetworkObject objNet)) {
            //    Debug.Log("Got networkObject");
            //    objNet.Spawn(true); // faz o objeto aparecer de forma sincronizada
            //    Debug.Log("Spawned");
            //}
            //else {
            //    Debug.Log("Did not get networkObject");
            //}
        }

        Debug.Log("Got object from pooling");

        obj.SetActive(false);
        return obj; // retorna um objeto 
    }

    public void ReturnObjetToQueue(GameObject objectToReturn) {
        if (!IsServer) return;

        //if(objectToReturn.TryGetComponent<NetworkObject>(out NetworkObject objNet)) {
        //    objNet.Despawn();
        //}
        objectToReturn.SetActive(false);
        poolingDictioinary[objectToReturn].Enqueue(objectToReturn);
    }
}
