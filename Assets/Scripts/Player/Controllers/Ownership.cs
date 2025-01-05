using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Ownership : NetworkBehaviour {
    #region Variables
    [Header("Character")]
    [SerializeField] Characters character;
    [SerializeField] MonoBehaviour[] scriptsToBeTurnedOff;

    [Header("Camera")]
    [SerializeField] CinemachineCamera cameraCineMachine;

    NetworkObject _netWorkObject;
    #endregion

    #region Methods
    void Start() {
        _netWorkObject = GetComponent<NetworkObject>();

        if (NetworkManager.Singleton.IsHost) {
            AssignOwnership();
        }

        ConfigureObjectForLocalPlayer();
    }
    void AssignOwnership() {
        if (LocalWhiteBoard.Instance.IsSinglePlayer) return;
        else StartCoroutine(AssignOwnerMultiplayer());
    }
    private IEnumerator AssignOwnerMultiplayer() {
        while (NetworkManager.Singleton.ConnectedClientsList.Count < 2) {
            yield return null;
        }

        // Ownership no modo multiplayer
        if (WhiteBoard.Singleton.PlayerOneCharacter.Value == character) {
            _netWorkObject.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
        }
        else if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == character) {
            _netWorkObject.ChangeOwnership(NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
        }

    }

    private void ConfigureObjectForLocalPlayer() {
        // Configura câmera e input baseados no ownership e no personagem local
        if (LocalWhiteBoard.Instance.PlayerCharacter == this.character) {
            SetFollowCamera();
            //EnablePlayerControl();
        }
        else {
            DisablePlayerControl();
        }
    }

    private void SetFollowCamera() {
        cameraCineMachine.Follow = transform;
    }

    //private void EnablePlayerControl() {
    //    _input.enabled = true;
    //    GetComponent<PlayerController>().enabled = true;
    //}

    private void DisablePlayerControl() {
        //_input.enabled = false;
        //GetComponent<PlayerController>().enabled = false;
        foreach(var s in scriptsToBeTurnedOff) {
            s.enabled = false;
        }
    }
    #endregion
}
