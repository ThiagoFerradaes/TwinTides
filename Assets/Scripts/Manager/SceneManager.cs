using UnityEngine;
using Unity.Netcode;

public class SceneManager : NetworkBehaviour
{
    [SerializeField] GameObject maevisPreFab;
    [SerializeField] GameObject melPreFab;
    void Start()
    {
        if (IsServer) {
            SpawnPlayers();
        }
    }

    void SpawnPlayers() {

        int playerIndex = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList) {
            ulong clientID = client.ClientId;

            GameObject prefab = playerIndex == 0 ? maevisPreFab : melPreFab; 
            Vector3 prefabPos = Vector3.zero + new Vector3(playerIndex * 2,0,0);

            var playerObject = Instantiate(prefab, prefabPos, Quaternion.identity);

            var playerNetworkObject = playerObject.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnWithOwnership(clientID);

            playerIndex++;
        }
    }
}
