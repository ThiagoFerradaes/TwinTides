using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste : NetworkBehaviour {

    [SerializeField] GameObject prefab;

    private void Start() {
        if (IsHost) {
            InstantiateRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    void InstantiateRpc() {
        Instantiate(prefab);
    }
}
