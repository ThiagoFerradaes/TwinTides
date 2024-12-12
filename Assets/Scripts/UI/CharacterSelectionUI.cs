using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour {
    [SerializeField] bool playerOne;
    [SerializeField] Image characterImage;


    private void OnEnable() {
        ChangeCharacterUIClientRpc(Characters.Moly, Characters.Maevis);
        WhiteBoard.Singleton.PlayerOneCharacter.OnValueChanged += ChangeCharacterUIClientRpc;
        WhiteBoard.Singleton.PlayerTwoCharacter.OnValueChanged += ChangeCharacterUIClientRpc;
    }

    private void OnDisable() {
        if (WhiteBoard.Singleton != null) {
            WhiteBoard.Singleton.PlayerOneCharacter.OnValueChanged -= ChangeCharacterUIClientRpc;
            WhiteBoard.Singleton.PlayerTwoCharacter.OnValueChanged -= ChangeCharacterUIClientRpc;

        }
    }

    [ClientRpc]
    private void ChangeCharacterUIClientRpc(Characters preview, Characters newChar) {
        if (playerOne) {
            if (WhiteBoard.Singleton.PlayerOneCharacter.Value == Characters.Maevis) {
                characterImage.color = Color.red;
            }
            else {
                characterImage.color = Color.blue;
            }
        }
        else {
            if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == Characters.Maevis) {
                characterImage.color = Color.red;
            }
            else {
                characterImage.color = Color.blue;
            }
        }
    }
}
