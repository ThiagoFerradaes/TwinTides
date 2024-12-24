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
    Coroutine _setCameraCoroutine;

    NetworkObject _netWorkObject;
    void Start() {
        _netWorkObject = GetComponent<NetworkObject>();
        _input = GetComponent<PlayerInput>();

        if (NetworkManager.Singleton.IsHost) {
            StartCoroutine(nameof(DistributeOwnership));
        }

        _setCameraCoroutine = StartCoroutine(nameof(WaitToSetCamera));
        StartCoroutine(nameof(WaitToSetInputMap));
    }
    IEnumerator WaitToSetCamera() {
        while (!_netWorkObject.IsOwner) {
            yield return null;
            if (secondCharacterObject.GetComponent<NetworkBehaviour>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                break;
            }
        }
        SetFollowCamera();
    }
    private void SetFollowCamera() {
        if (NetworkManager.Singleton.LocalClientId == _netWorkObject.OwnerClientId) {
            cameraCineMachine.Follow = this.transform;
        }
    }
    IEnumerator WaitToSetInputMap() {
        while (!_netWorkObject.IsOwner) {
            yield return null;
            if (secondCharacterObject.GetComponent<NetworkBehaviour>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                break;
            }
        }
        SetInputMap();
    }
    void SetInputMap() {
        if (IsOwner) {
            _input.enabled = true;
        }
        else {
            _input.enabled = false;
        }
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
