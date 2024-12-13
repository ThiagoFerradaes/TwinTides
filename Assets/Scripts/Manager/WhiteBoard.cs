using Unity.Netcode;
using UnityEngine;

public enum Characters {
    Maevis,
    Moly
}
public class WhiteBoard: NetworkBehaviour
{
    public static WhiteBoard Singleton {  get; private set; }


    #region Variaveis
    public NetworkVariable<Characters> PlayerOneCharacter = new(Characters.Maevis);
    public NetworkVariable<Characters> PlayerTwoCharacter = new(Characters.Moly);

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
    }

    public override void OnDestroy() {
        base.OnDestroy();
        if (Singleton == this) {
            Singleton = null;
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChangeCharactersServerRpc(int player) {
        if (player == 1) {
            if (PlayerOneCharacter.Value == Characters.Maevis) {
                PlayerOneCharacter.Value = Characters.Moly;
            }
            else {
                PlayerOneCharacter.Value = Characters.Maevis;
            }
        }
        else {
            if (PlayerTwoCharacter.Value == Characters.Maevis) {
                PlayerTwoCharacter.Value = Characters.Moly;
            }
            else {
                PlayerTwoCharacter.Value = Characters.Maevis;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CharacterReadyServerRpc(int player) {
        if (player == 1) {
            if (PlayerOneReady.Value == true) { // Se vc ja ta pronto vc pode voltar
                PlayerOneReady.Value = false;
            }
            else { // Se vc n ta pronto, vc só pode dar pronto se o seu personagem for diferente do outro
                if (PlayerOneCharacter.Value != PlayerTwoCharacter.Value) {
                    PlayerOneReady.Value = true;
                }
                else {
                    Debug.Log("Personagem repetido");
                }
            }
        }
        if (player == 2) {
            if (PlayerTwoReady.Value == true) { // Se vc ja ta pronto vc pode voltar
                PlayerTwoReady.Value = false;
            }
            else { // Se vc n ta pronto, vc só pode dar pronto se o seu personagem for diferente do outro
                if (PlayerOneCharacter.Value != PlayerTwoCharacter.Value) {
                    PlayerTwoReady.Value = true;
                }
                else {
                    Debug.LogWarning("Personagem repetido");
                }
            }
        }
    }
    #endregion
}
