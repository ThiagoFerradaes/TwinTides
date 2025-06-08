using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayersDeathManager : NetworkBehaviour
{
    #region Variables

    [SerializeField] Image blackOut;
    [SerializeField] GameObject defeatScreen;
    [SerializeField] TextMeshProUGUI reviveVotedBox;
    [SerializeField] float positionOffSet;
    List<HealthManager> listOfPlayers = new();

    NetworkVariable<int> playersVoted = new();
    NetworkVariable<int> playersDead = new();

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
        foreach(var player in SceneManager.ActivePlayers.Values) {
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
        Vector3 lastCheckPointPosition = CheckPointsManager.Instance.ReturnLastTotemPosition();

        for (int i = 0; i < listOfPlayers.Count; i ++) {
            listOfPlayers[i].ReviveHandler(100);
            listOfPlayers[i].gameObject.transform.position = lastCheckPointPosition + new Vector3(positionOffSet * i, 0, positionOffSet * i);
            listOfPlayers[i].gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        defeatScreen.SetActive(false);
        Time.timeScale = 1f;
    }
    #endregion
}
