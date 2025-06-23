using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Video;

public class TutorialManager : NetworkBehaviour {
    [Header("Captions")]
    [SerializeField] Sprite movementCaption;
    [SerializeField] Sprite baseAttackCaption;
    [SerializeField] Sprite aimModeCaption;
    [SerializeField] Sprite commonRelicCaption;
    [SerializeField] Sprite legendaryRelicCaption;
    [SerializeField] Sprite interactCaption;

    [Header("Videos")]
    [SerializeField] VideoClip movementVideo;
    [SerializeField] VideoClip baseAttackVideo;
    [SerializeField] VideoClip aimModeVideo;
    [SerializeField] VideoClip commonRelicVideo;
    [SerializeField] VideoClip legendaryRelicVideo;
    [SerializeField] VideoClip interactVideo;

    [Header("Buttons")]
    [SerializeField] Button closeButton;
    [SerializeField] Button leftArrowButton;
    [SerializeField] Button rightArrowButton;

    [Header("Components")]
    [SerializeField] Image captionImage;
    [SerializeField] TextMeshProUGUI playersVoteToCloseText;
    [SerializeField] VideoPlayer tutorialVideoPlayer;

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
    private void UpdateTutorialPageUI() {
        switch (tutorialPageIndex) {
            case 1:
                captionImage.sprite = movementCaption;
                tutorialVideoPlayer.clip = movementVideo;
                leftArrowButton.gameObject.SetActive(false);
                break;
            case 2:
                captionImage.sprite = baseAttackCaption;
                tutorialVideoPlayer.clip = baseAttackVideo;
                leftArrowButton.gameObject.SetActive(true);
                break;
            case 3:
                captionImage.sprite = aimModeCaption;
                tutorialVideoPlayer.clip = aimModeVideo;
                break;
            case 4:
                captionImage.sprite = commonRelicCaption;
                tutorialVideoPlayer.clip = commonRelicVideo;
                break;
            case 5:
                captionImage.sprite = legendaryRelicCaption;
                tutorialVideoPlayer.clip = legendaryRelicVideo;
                rightArrowButton.gameObject.SetActive(true);
                break;
            case 6:
                captionImage.sprite = interactCaption;
                tutorialVideoPlayer.clip = interactVideo;
                rightArrowButton.gameObject.SetActive(false);
                break;
        }

        PlayTutorialVideo();
    }

    private void PlayTutorialVideo() {
        if (tutorialVideoPlayer.clip == null) return;

        tutorialVideoPlayer.Stop();
        tutorialVideoPlayer.isLooping = true;
        tutorialVideoPlayer.Play();
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
