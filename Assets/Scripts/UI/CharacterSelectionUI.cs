using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour {
    [SerializeField] bool playerOne;
    [SerializeField] Image characterImage;
    [SerializeField] Image readyImage;
    [SerializeField] Sprite maevisSprite, melSprite, readySpriteOne, notReadySpriteOne, readySpriteTwo, notReadySpriteTwo;


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
                characterImage.sprite = maevisSprite;
            }
            else {
                characterImage.sprite = melSprite;
            }
        }
        else {
            if (WhiteBoard.Singleton.PlayerTwoCharacter.Value == Characters.Maevis) {
                characterImage.sprite = maevisSprite;
            }
            else {
                characterImage.sprite = melSprite;
            }
        }
    }

    [ClientRpc]
    void ChangeReadyTextClientRpc(bool old, bool newBool) {
        if (playerOne) {
            readyImage.sprite = WhiteBoard.Singleton.PlayerOneReady.Value ? readySpriteOne : notReadySpriteOne;
        }
        else {
            readyImage.sprite = WhiteBoard.Singleton.PlayerOneReady.Value ? readySpriteTwo : notReadySpriteTwo;
        }
    }
}
