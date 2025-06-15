using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

public class SceneManager : NetworkBehaviour
{

    public static Dictionary<Characters ,GameObject> ActivePlayers = new();

    [SerializeField] GameObject maevisPreFab;
    [SerializeField] GameObject melPreFab;
    [SerializeField] Transform originPos;

    public static event Action OnPlayersSpawned;
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
            Characters typeOfCharacter = prefab == maevisPreFab ? Characters.Maevis : Characters.Mel;
            Vector3 prefabPos = originPos.position + new Vector3(playerIndex * 3f, 0f, 0f);

            prefabPos.y = GetFloorHeight(prefabPos) + 1f;

            var playerObject = Instantiate(prefab, prefabPos, Quaternion.identity);

            var playerNetworkObject = playerObject.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnWithOwnership(clientID, true);

            ActivePlayers[typeOfCharacter] = (playerObject);

            playerIndex++;
        }

        OnPlayersSpawned?.Invoke();
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

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }
}
