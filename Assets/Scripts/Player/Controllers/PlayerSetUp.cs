using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour {
    #region Variables
    [SerializeField] MonoBehaviour[] scriptsToBeTurnedOff;

    public Characters _character;

    public static event Action<GameObject> OnPlayerSpawned;
    #endregion

    #region Methods
    void Start() {
        ConfigureObjectForLocalPlayer();
    }

    private void ConfigureObjectForLocalPlayer() {
        if (LocalWhiteBoard.Instance.PlayerCharacter == _character) {
            OnPlayerSpawned?.Invoke(this.gameObject);
            Debug.Log("Owned: " +  gameObject.name);    
        }
        else {   
            Debug.Log("Not owned: " + gameObject.name);
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
