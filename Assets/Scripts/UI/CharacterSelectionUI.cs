using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour {
    [SerializeField] bool playerOne;
    [SerializeField] Image characterImage, nameImage;
    [SerializeField] Image readyImage, readyButtonImage;
    [SerializeField] Sprite maevisSprite, melSprite, maevisNameSprite, melNameSprite, readyButtonSpriteOn, readyButtonSpriteOff;


    private void OnEnable() {

        if (playerOne)ChangeCharacterUIClientRpc(Characters.Mel, WhiteBoard.Singleton.PlayerOneCharacter.Value);
        else ChangeCharacterUIClientRpc(Characters.Maevis, WhiteBoard.Singleton.PlayerTwoCharacter.Value);

        ChangeReadyTextClientRpc(false, false);

        if (playerOne) WhiteBoard.Singleton.PlayerOneCharacter.OnValueChanged += ChangeCharacterUIClientRpc;
        else WhiteBoard.Singleton.PlayerTwoCharacter.OnValueChanged += ChangeCharacterUIClientRpc;

        if (playerOne) WhiteBoard.Singleton.PlayerOneReady.OnValueChanged += ChangeReadyTextClientRpc;
        else WhiteBoard.Singleton.PlayerTwoReady.OnValueChanged += ChangeReadyTextClientRpc; 
    }

    private void OnDisable() {
        if (WhiteBoard.Singleton != null) {
            if (playerOne) WhiteBoard.Singleton.PlayerOneCharacter.OnValueChanged -= ChangeCharacterUIClientRpc;
            else WhiteBoard.Singleton.PlayerTwoCharacter.OnValueChanged -= ChangeCharacterUIClientRpc;

            if (playerOne) WhiteBoard.Singleton.PlayerOneReady.OnValueChanged -= ChangeReadyTextClientRpc;
            else WhiteBoard.Singleton.PlayerTwoReady.OnValueChanged -= ChangeReadyTextClientRpc;

        }
    }

    [ClientRpc]
    private void ChangeCharacterUIClientRpc(Characters preview, Characters newChar) {
        characterImage.sprite = newChar == Characters.Maevis ? maevisSprite : melSprite;
        nameImage.sprite = newChar == Characters.Maevis ? maevisNameSprite : melNameSprite;

    }

    [ClientRpc]
    void ChangeReadyTextClientRpc(bool old, bool newBool) {
        readyImage.gameObject.SetActive(newBool);
        readyButtonImage.sprite = newBool ? readyButtonSpriteOff : readyButtonSpriteOn;
    }
}
