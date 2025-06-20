using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialManager : NetworkBehaviour {
    [Header("Captions")]
    [SerializeField] Sprite movementCaption;
    [SerializeField] Sprite baseAttackCaption;
    [SerializeField] Sprite aimModeCaption;
    [SerializeField] Sprite commonRelicCaption;
    [SerializeField] Sprite legendaryRelicCaption;
    [SerializeField] Sprite interactCaption;

    [Header("Buttons")]
    [SerializeField] Button closeButton;
    [SerializeField] Button leftArrowButton;
    [SerializeField] Button rightArrowButton;

    [Header("Components")]
    [SerializeField] Image captionImage;
    [SerializeField] TextMeshProUGUI playersVoteToCloseText;

    int tutorialPageIndex = 1;
    readonly NetworkVariable<int> playerVotedToCloseTutorial = new();

    public static event Action OnTutorialClosed;
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        Time.timeScale = 0f;

        LocalWhiteBoard.Instance.AnimationOn = true;

        SetButtons();

        playerVotedToCloseTutorial.OnValueChanged += UpdatePlayersVotedText;

        SetInitialScreen();
    }

    void SetButtons() {
        leftArrowButton.onClick.AddListener(LeftArrowFunction);
        rightArrowButton.onClick.AddListener(RightArrowFunction);
        closeButton.onClick.AddListener(CrossFunction);
    }

    void SetInitialScreen() {
        tutorialPageIndex = 1;
        UpdateTutorialPageUI();
        UpdatePlayersVotedText(0, 0);
    }

    #region Buttons
    public void LeftArrowFunction() {
        tutorialPageIndex--;
        tutorialPageIndex = Mathf.Max(tutorialPageIndex, 1);

        UpdateTutorialPageUI();
    }
    public void RightArrowFunction() {
        tutorialPageIndex++;
        tutorialPageIndex = Mathf.Min(tutorialPageIndex, 6);

        UpdateTutorialPageUI();
    }

    public void CrossFunction() {
        closeButton.interactable = false;
        VoteToCloseTutorialRpc();
    }
    #endregion

    #region Functions
    void UpdateTutorialPageUI() {
        switch (tutorialPageIndex) {
            case 1:
                captionImage.sprite = movementCaption;
                leftArrowButton.gameObject.SetActive(false);
                break;
            case 2:
                captionImage.sprite = baseAttackCaption;
                leftArrowButton.gameObject.SetActive(true);
                break;
            case 3:
                captionImage.sprite = aimModeCaption;
                break;
            case 4:
                captionImage.sprite = commonRelicCaption;
                break;
            case 5:
                captionImage.sprite = legendaryRelicCaption;
                rightArrowButton.gameObject.SetActive(true);
                break;
            case 6:
                captionImage.sprite = interactCaption;
                rightArrowButton.gameObject.SetActive(false);
                break;
        }
    }

    [Rpc(SendTo.Server)]
    void VoteToCloseTutorialRpc() {
        playerVotedToCloseTutorial.Value++;

        if (playerVotedToCloseTutorial.Value >= NetworkManager.ConnectedClientsList.Count) CloseTutorialRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void CloseTutorialRpc() {
        Time.timeScale = 1f;
        LocalWhiteBoard.Instance.AnimationOn = false;
        OnTutorialClosed?.Invoke();
        gameObject.SetActive(false);
    }

    void UpdatePlayersVotedText(int oldValue, int newValue) {
        playersVoteToCloseText.text = $" {newValue} / {NetworkManager.ConnectedClientsList.Count}";
    }
    #endregion
}
