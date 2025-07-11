using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BlackBeardManager : NetworkBehaviour {
    [SerializeField] BlackBeardMachineState blackBeard;
    [SerializeField] BlackBeardHealthUiManager UI;
    HashSet<HealthManager> _listOfPlayers = new();
    bool hasStarted;

    public override void OnNetworkSpawn() {
        BlackBeardMachineState.OnDeath += BlackBeardDeath;
    }

    private void BlackBeardDeath() {
        UI.TurnUIOff();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        Debug.Log(health.name);

        _listOfPlayers.Add(health);

        Debug.Log("List count: " + _listOfPlayers.Count);

        if (_listOfPlayers.Count >= NetworkManager.Singleton.ConnectedClientsList.Count && !hasStarted && IsServer) StartFightRpc();
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Remove(health);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void StartFightRpc() {
        Debug.Log("Start RPC");
        hasStarted = true;
        UI.TurnUIOn();
        blackBeard.StartFight();
    }
}
