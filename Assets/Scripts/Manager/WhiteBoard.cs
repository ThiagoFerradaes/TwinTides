using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WhiteBoard: NetworkBehaviour
{
    public static WhiteBoard Singleton {  get; private set; }


    #region Variaveis
    public NetworkVariable<Characters> PlayerOneCharacter = new(Characters.Maevis);
    public NetworkVariable<Characters> PlayerTwoCharacter = new(Characters.Mel);

    public NetworkVariable<bool> PlayerOneReady = new(false);
    public NetworkVariable<bool> PlayerTwoReady = new(false);

    #endregion
    #region Métodos

    private void Awake() {
        if (Singleton != null && Singleton != this) {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        if (Singleton == this) {
            Singleton = null;
        }
    }
    public void ChangeCharacters(int player) {
        if (!IsServer) return;
        if (player == 1) {
            PlayerOneCharacter.Value = PlayerOneCharacter.Value == Characters.Mel ? Characters.Maevis : Characters.Mel;
        }
        else {
            PlayerTwoCharacter.Value = PlayerTwoCharacter.Value == Characters.Mel ? Characters.Maevis : Characters.Mel;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CharacterReadyServerRpc(int player) {
        if (player == 1) {
            PlayerOneReady.Value = !PlayerOneReady.Value;
        }
        else {
            PlayerTwoReady.Value = !PlayerTwoReady.Value;
        }
    }

    [ServerRpc (RequireOwnership =false)]
    public void ResetWhiteBoardServerRpc() {
        PlayerOneReady.Value = false;
        PlayerTwoReady.Value = false;

        PlayerOneCharacter.Value = Characters.Maevis;
        PlayerTwoCharacter.Value = Characters.Mel;
    }
    #endregion
}
