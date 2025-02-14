using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour {
    #region Variables
    [SerializeField] MonoBehaviour[] scriptsToBeTurnedOff;
    public GameObject CameraObject;
    public Characters Character;

    public static event Action<GameObject> OnPlayerSpawned, OnPlayerTwoSpawned;
    #endregion

    #region Methods
    void Start() {
        ConfigureObjectForLocalPlayer();
    }

    private void ConfigureObjectForLocalPlayer() {
        if (LocalWhiteBoard.Instance.PlayerCharacter == Character) {
            OnPlayerSpawned?.Invoke(this.gameObject);   
        }
        else {   
            OnPlayerTwoSpawned?.Invoke(this.gameObject);
            DisablePlayerControl();
        }
    }

    private void DisablePlayerControl() {
        foreach(var s in scriptsToBeTurnedOff) {
            s.enabled = false;
        }
    }
    #endregion
}
