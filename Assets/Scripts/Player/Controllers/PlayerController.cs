using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour {

    [Header("Ownership")]
    [SerializeField] private Characters character;
    NetworkObject _netWorkObject;

    [Header("Camera")]
    [SerializeField] private GameObject shoulder;
    [SerializeField] CinemachineCamera cameraOverTheShoulder;

    [Header("Movement")]
    public float CharacterMoveSpeed;
    void Start() {
        _netWorkObject = GetComponent<NetworkObject>();

        if (NetworkManager.Singleton.IsHost) {
            StartCoroutine(nameof(DistributeOwnership));
        }

        StartCoroutine(nameof(WaitToSetCamera));
    }
    IEnumerator WaitToSetCamera() {
        while (!_netWorkObject.IsOwner) {
            yield return null;
        }
        Debug.Log(_netWorkObject.name);
        SetFollowCamera();
    }

    private void SetFollowCamera() {
        if (NetworkManager.Singleton.LocalClientId == _netWorkObject.OwnerClientId) {
            cameraOverTheShoulder.Follow = shoulder.transform;
        }
    }

    private IEnumerator DistributeOwnership() {

        while (NetworkManager.Singleton.ConnectedClientsList.Count < 2) {
            yield return null;
            Debug.Log("Waiting");
        }

        if (WhiteBoard.Singleton.PlayerOneCharacter.Value == character) {
            _netWorkObject.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
            Debug.Log("Player 1: " + WhiteBoard.Singleton.PlayerOneCharacter.Value);
        }
        else if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == character) {
            _netWorkObject.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
            Debug.Log("Player 2: " + WhiteBoard.Singleton.PlayerTwoCharacter.Value);
        }


    }

    void FixedUpdate() {
        MovementInputs();
    }

    private void MovementInputs() {
        if (Keyboard.current.wKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.forward.normalized);
        }
        else if (Keyboard.current.sKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.back.normalized);
        }
        else if (Keyboard.current.dKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.right.normalized);
        }
        else if (Keyboard.current.aKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.left.normalized);
        }
    }
}
