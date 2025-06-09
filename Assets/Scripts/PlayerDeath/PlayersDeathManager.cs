using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayersDeathManager : NetworkBehaviour {
    #region Variables

    [SerializeField] Image blackOut;
    [SerializeField] GameObject defeatScreen;
    [SerializeField] TextMeshProUGUI reviveVotedBox;
    [SerializeField] float positionOffSet;
    [SerializeField] float blackOutTimeToExpand;
    [SerializeField] float blackOutTimeToContract;
    [SerializeField] float blackOutDuration;
    [SerializeField] CinemachineCamera cam;
    List<HealthManager> listOfPlayers = new();

    NetworkVariable<int> playersVoted = new();
    NetworkVariable<int> playersDead = new();

    public static event Action OnGameRestart;
    public static event Action OnDefeat;

    [SerializeField] Texture2D normalMouseSprite;
    #endregion

    #region Initialize
    void Start() {
        if (SceneManager.ActivePlayers.Count == 0) {
            SceneManager.OnPlayersSpawned += Init;
        }
        else {
            Init();
        }

        playersVoted.OnValueChanged += ChangePlayersVotedText;
    }

    void Init() {
        foreach (var player in SceneManager.ActivePlayers.Values) {
            HealthManager health = player.GetComponent<HealthManager>();
            health.OnDeath += PlayerDeath;
            health.OnRevive += PlayerRevived;
            listOfPlayers.Add(health);
        }
    }
    public override void OnDestroy() {
        base.OnDestroy();

        SceneManager.OnPlayersSpawned -= Init;

        foreach (var player in listOfPlayers) {
            player.OnDeath -= PlayerDeath;
            player.OnRevive -= PlayerRevived;
        }
    }

    #endregion

    #region Death
    void PlayerRevived() {
        if (IsServer) {
            playersDead.Value--;
            playersDead.Value = Mathf.Max(playersDead.Value, 0);
        }
    }
    private void PlayerDeath() {
        if (IsServer) {
            playersDead.Value++;
            playersDead.Value = Mathf.Min(playersDead.Value, NetworkManager.Singleton.ConnectedClientsList.Count);
            if (playersDead.Value >= NetworkManager.Singleton.ConnectedClientsList.Count) {
                playersDead.Value = 0;
                DefeatRpc();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DefeatRpc() {
        Time.timeScale = 0;
        defeatScreen.SetActive(true);
        OnDefeat?.Invoke();
        Cursor.SetCursor(normalMouseSprite, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    #region Revive
    public void ReviveButton() {
        VoteRpc();
    }

    [Rpc(SendTo.Server)]
    void VoteRpc() {
        playersVoted.Value++;
        if (playersVoted.Value >= NetworkManager.Singleton.ConnectedClientsList.Count) {
            ReviveRpc();
            playersVoted.Value = 0;
        }
    }
    void ChangePlayersVotedText(int old, int newInt) {
        reviveVotedBox.text = $"Revive: {playersVoted.Value} / {NetworkManager.Singleton.ConnectedClientsList.Count}";
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ReviveRpc() {
        StartCoroutine(ReviveRoutine());
    }

    IEnumerator ReviveRoutine() {
        defeatScreen.SetActive(false);

        Tween expandTween = ExpandFromCenter();
        yield return expandTween.WaitForCompletion();

        OnGameRestart?.Invoke();

        Vector3 lastCheckPointPosition = CheckPointsManager.Instance.ReturnLastTotemPosition();

        for (int i = 0; i < listOfPlayers.Count; i++) {
            listOfPlayers[i].ReviveHandler(100);
            listOfPlayers[i].gameObject.transform.position = lastCheckPointPosition + new Vector3(positionOffSet * i, 0, positionOffSet * i);
            listOfPlayers[i].gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            listOfPlayers[i].GetComponent<PlayerController>().isAiming = false;
        }

        cam.OnTargetObjectWarped(cam.Follow, cam.Follow.position);

        yield return new WaitForSecondsRealtime(blackOutDuration);

        Tween contractTween = blackOut.rectTransform.DOSizeDelta(Vector2.zero, blackOutTimeToContract).SetEase(Ease.InCubic).SetUpdate(true);

        yield return contractTween.WaitForCompletion();

        blackOut.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }


    public Tween ExpandFromCenter() {
        blackOut.rectTransform.sizeDelta = Vector2.zero;
        blackOut.gameObject.SetActive(true);
        blackOut.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        blackOut.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        blackOut.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        blackOut.rectTransform.anchoredPosition = Vector2.zero;

        // Começa pequeno
        blackOut.rectTransform.sizeDelta = Vector2.zero;

        // Expande para preencher a tela (por exemplo 1920x1080, ou use Screen.width/height)
        return blackOut.rectTransform.DOSizeDelta(new Vector2(Screen.width, Screen.height), blackOutTimeToExpand).SetEase(Ease.OutCubic).SetUpdate(true);
    }
    #endregion
}
