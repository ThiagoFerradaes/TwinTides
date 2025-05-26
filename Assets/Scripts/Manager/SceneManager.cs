using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class SceneManager : NetworkBehaviour
{

    public static List<GameObject> ActivePlayers = new();

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

            GameObject prefab = GetPrefab(playerIndex);
            Vector3 prefabPos = Vector3.zero + new Vector3(playerIndex * 2,8.1f,0);

            var playerObject = Instantiate(prefab, prefabPos, Quaternion.identity);

            var playerNetworkObject = playerObject.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnWithOwnership(clientID, true);

            ActivePlayers.Add(playerObject);

            playerIndex++;
        }
    }
    GameObject GetPrefab(int playerIndex) {
        if (playerIndex == 0) {
            if (WhiteBoard.Singleton.PlayerOneCharacter.Value == Characters.Maevis) {
                return maevisPreFab;
            }
            else {
                return melPreFab;
            }
        }
        else {
            if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == Characters.Maevis) {
                return maevisPreFab;
            }
            else {
                return melPreFab;
            }
        }
    }
}
