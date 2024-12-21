using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour {

    [Header("Ownership")]
    [SerializeField] private Characters character;
    NetworkObject _netWorkObject;

    [Header("Camera")]
    [SerializeField] CinemachineCamera cameraOverTheShoulder;
    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;

    [Header("Movement")]
    public float CharacterMoveSpeed;
    [SerializeField] float rotationSpeed;
    bool _isMoving;
    float _rotationY;
    void Start() {
        _netWorkObject = GetComponent<NetworkObject>();

        if (NetworkManager.Singleton.IsHost) {
            StartCoroutine(nameof(DistributeOwnership));
        }

        StartCoroutine(nameof(WaitToSetCamera));

        _cameraInputController = cameraOverTheShoulder.GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = cameraOverTheShoulder.GetComponent<CinemachineOrbitalFollow>();
    }
    IEnumerator WaitToSetCamera() {
        while (!_netWorkObject.IsOwner) {
            yield return null;
        }
        SetFollowCamera();
    }

    private void SetFollowCamera() {
        if (NetworkManager.Singleton.LocalClientId == _netWorkObject.OwnerClientId) {
            cameraOverTheShoulder.Follow = this.transform;
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

    void FixedUpdate() {
        MovementInputs();
    }

    private void MovementInputs() {
        Move();
        Rotate();

    }

    private void Move() {
        if (Keyboard.current.wKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.forward.normalized);
            _isMoving = true;
        }
        else if (Keyboard.current.sKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.back.normalized);
            _isMoving = true;
        }
        else if (Keyboard.current.dKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.right.normalized);
            _isMoving = true;
        }
        else if (Keyboard.current.aKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.left.normalized);
            _isMoving = true;
        }
    }

    void Rotate() {
        if (!_isMoving) { // ta parado
            _cameraInputController.enabled = true;
        }
        else { // ta se movendo
            _cameraInputController.enabled = false;
            _cameraOrbital.HorizontalAxis.TriggerRecentering();

            float mouseX = Input.GetAxis("Mouse X");

            _rotationY += mouseX * rotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);

        }
    }
}
