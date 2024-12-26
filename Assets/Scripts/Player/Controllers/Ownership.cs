using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Ownership : NetworkBehaviour
{
    [Header("Character")]
    [SerializeField] Characters character;
    [SerializeField] GameObject secondCharacterObject;
    PlayerInput _input;

    [Header("Camera")]
    [SerializeField] CinemachineCamera cameraCineMachine;

    NetworkObject _netWorkObject;
    void Start() {
        _netWorkObject = GetComponent<NetworkObject>();
        _input = GetComponent<PlayerInput>();

        if (NetworkManager.Singleton.IsHost) {
            StartCoroutine(nameof(DistributeOwnership));
        }

        StartCoroutine(nameof(WaitToSetCamera));
    }
    IEnumerator WaitToSetCamera() {
        while (!_netWorkObject.IsOwner) {
            yield return null;
            if (secondCharacterObject.GetComponent<NetworkBehaviour>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                break;
            }
        }
        SetFollowCamera();
        SetInputMap();
    }
    private void SetFollowCamera() {
        if (NetworkManager.Singleton.LocalClientId == _netWorkObject.OwnerClientId) {
            cameraCineMachine.Follow = this.transform;
        }
    }
    void SetInputMap() {
        if (IsOwner) {
            _input.enabled = true;
        }
        else {
            DisableScripts(); // Desligando scripts do objeto que não tem dono local
        }
    }
    void DisableScripts() {
        _input.enabled = false; // Desligando o player input
        GetComponent<PlayerController>().enabled = false; // Desligando o PlayerController
    }
    private IEnumerator DistributeOwnership() {

        while (NetworkManager.Singleton.ConnectedClientsList.Count < 2) {
            yield return null;
        }

        if (WhiteBoard.Singleton.PlayerOneCharacter.Value == character) {
            _netWorkObject.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
        }
        else if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == character) {
            _netWorkObject.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
        }
    }
}
