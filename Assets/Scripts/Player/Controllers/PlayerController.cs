using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour {

    [Header("Ownership")]
    [SerializeField] private Characters character;

    [Header("Movement")]
    public float CharacterMoveSpeed;
    void Start() {
        if (NetworkManager.Singleton.IsHost) {
            NetworkManager.SceneManager.OnLoadComplete += DistributeOwnership;
        }
    }

    private void DistributeOwnership(ulong clientId, string sceneName, LoadSceneMode loadSceneMode) {
        Debug.Log("Entrei aqui");
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
            NetworkObject obj = GetComponent<NetworkObject>();
            Debug.Log("Jogadores conectados");
            if (WhiteBoard.Singleton.PlayerOneCharacter.Value == character) {
                obj.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
                Debug.Log("Player One: " + NetworkManager.Singleton.ConnectedClientsList[0].ClientId
                    + WhiteBoard.Singleton.PlayerOneCharacter.Value);
            }

            if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == character) {
                obj.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
                Debug.Log("Player Two: " + NetworkManager.Singleton.ConnectedClientsList[1].ClientId
                    + WhiteBoard.Singleton.PlayerTwoCharacter.Value);
            }

            NetworkManager.SceneManager.OnLoadComplete -= DistributeOwnership;
        }
    }

    void FixedUpdate() {
            MovementInputs();
    }

    private void MovementInputs() {
        if (Keyboard.current.wKey.isPressed) {
            transform.position += new Vector3(0, 0, CharacterMoveSpeed);
        }
        else if (Keyboard.current.sKey.isPressed) {
            transform.position += new Vector3(0, 0, -CharacterMoveSpeed);
        }
        else if (Keyboard.current.dKey.isPressed) {
            transform.position += new Vector3(CharacterMoveSpeed, 0, 0);
        }
        else if (Keyboard.current.aKey.isPressed) {
            transform.position += new Vector3(-CharacterMoveSpeed, 0, 0);
        }
    }
}
