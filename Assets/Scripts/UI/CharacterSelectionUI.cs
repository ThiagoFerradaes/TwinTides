using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour {
    [SerializeField] bool playerOne;
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI readyText;


    private void OnEnable() {
        ChangeCharacterUIClientRpc(Characters.Mel, Characters.Maevis);
        ChangeReadyTextClientRpc(false, false);
        WhiteBoard.Singleton.PlayerOneCharacter.OnValueChanged += ChangeCharacterUIClientRpc;
        WhiteBoard.Singleton.PlayerTwoCharacter.OnValueChanged += ChangeCharacterUIClientRpc;

        WhiteBoard.Singleton.PlayerOneReady.OnValueChanged += ChangeReadyTextClientRpc;
        WhiteBoard.Singleton.PlayerTwoReady.OnValueChanged += ChangeReadyTextClientRpc;
    }

    private void OnDisable() {
        if (WhiteBoard.Singleton != null) {
            WhiteBoard.Singleton.PlayerOneCharacter.OnValueChanged -= ChangeCharacterUIClientRpc;
            WhiteBoard.Singleton.PlayerTwoCharacter.OnValueChanged -= ChangeCharacterUIClientRpc;

            WhiteBoard.Singleton.PlayerOneReady.OnValueChanged -= ChangeReadyTextClientRpc;
            WhiteBoard.Singleton.PlayerTwoReady.OnValueChanged -= ChangeReadyTextClientRpc;

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

    [ClientRpc]
    void ChangeReadyTextClientRpc(bool old, bool newBool) {
        if (playerOne) {
            readyText.text = WhiteBoard.Singleton.PlayerOneReady.Value ? "Ready" : "Not Ready";
        }
        else {
            readyText.text = WhiteBoard.Singleton.PlayerTwoReady.Value ? "Ready" : "Not Ready";
        }
    }
}
