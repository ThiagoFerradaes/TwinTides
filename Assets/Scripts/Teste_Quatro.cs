using Unity.Netcode;
using UnityEngine;

public abstract class Teste_Quatro : NetworkBehaviour
{
    void Start() {
        TestRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestRpc() {
        Debug.Log("Hello");
    }

    public abstract void Funcao();
}
