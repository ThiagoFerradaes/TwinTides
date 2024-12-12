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
    #endregion
}
