using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class KeyLockScreen : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button closeButton;
    [SerializeField] Button giveKeysButton;

    [Header("Components")]
    [SerializeField] Image characterImage;
    [SerializeField] Image title;
    [SerializeField] GameObject screen;
    [SerializeField] TextMeshProUGUI keysText;

    [Header("Sprites")]
    [SerializeField] Sprite maevisSprite;
    [SerializeField] Sprite melSprite;

    bool isMaevis;

    public static event Action<bool> OnGiveAllKeys;

    #region Initialize
    private void Start() {
        SetButtons();
        KeyLockTotem.OnTurnScreenOn += TurnScreenOn;
    }
    private void OnDestroy() {
        KeyLockTotem.OnTurnScreenOn -= TurnScreenOn;
    }
    #endregion

    #region Buttons
    void SetButtons() {
        giveKeysButton.onClick.AddListener(GiveKeysButton);
        closeButton.onClick.AddListener(CloseButton);
    }

    void CloseButton() {
        screen.SetActive(false);
        LocalWhiteBoard.Instance.AnimationOn = false;
    }

    void GiveKeysButton() {
        giveKeysButton.interactable = false;
        keysText.text = " 3 / 3";
        LocalWhiteBoard.Instance.UseAllKeys();
        OnGiveAllKeys?.Invoke(isMaevis);
    }
    #endregion

    void TurnScreenOn(bool isMaevis, bool hasAllKeys) {
        this.isMaevis = isMaevis;
        characterImage.sprite = isMaevis ? maevisSprite : melSprite;
        keysText.text = hasAllKeys ? " 3 / 3" : " 0 / 3";
        giveKeysButton.interactable = CanGiveKeys();

        LocalWhiteBoard.Instance.AnimationOn = true;
        screen.SetActive(true);
    }
    bool CanGiveKeys() {
        bool isCorrectTotem = (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) == isMaevis;
        bool hasAllKeys = LocalWhiteBoard.Instance.ReturnAmountOfKeys() == 3;
        bool hasNotGivenKeysYet = !LocalWhiteBoard.Instance.ReturnFinalDoorOpened();

        return isCorrectTotem && hasAllKeys && hasNotGivenKeysYet;
    }
}
