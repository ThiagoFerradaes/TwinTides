using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Teste_Tres : NetworkBehaviour {

    private void OnEnable() {
        FunctionRpc();
    }

    [Rpc(SendTo.Everyone)]
    void FunctionRpc() {
        Debug.Log("Ola");
    }
}
