using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : NetworkBehaviour {

    [Header("Change scene")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] float loadingTime;
    [SerializeField] string nomeDaCena;

    [Header("Lobby")]
    [SerializeField] GameObject hostVisualIndicador;
    [SerializeField] GameObject clientVisualIndicador;
    [SerializeField] TextMeshProUGUI hostTMP;
    [SerializeField] TextMeshProUGUI clientTMP;
    [SerializeField] GameObject lobbyClosedScreen;
    [SerializeField] GameObject lobbyScreen;
    [SerializeField] GameObject changeCharacterOnButton;
    [SerializeField] GameObject changeCHaracterTwoButton;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject[] readyButton;
    [SerializeField] TextMeshProUGUI[] readyTexts;
    ulong hostId;

    private void Start() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconneted;
        }

    }

    void OnDisable() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconneted;
        }
    }

    #region Funções De Entrada no Lobby
    private void OnClientConnected(ulong obj) {

        if (NetworkManager.Singleton.IsHost) {
            switch (NetworkManager.Singleton.ConnectedClients.Count) {
                case 0:
                    break;
                case 1:
                    HostUpdateVisualsClientRpc(obj, true);
                    break;
                case 2:
                    HostUpdateVisualsClientRpc(obj, false);
                    break;
            }
        }
        else {
            EachPlayerButtonsOn(false);
        }
    }

    [ClientRpc]
    void HostUpdateVisualsClientRpc(ulong clientId, bool isHost) {
        if (isHost) {
            hostId = clientId;
            hostVisualIndicador.SetActive(true);
            hostTMP.text = $"Player 1: {hostId}";
            EachPlayerButtonsOn(true);
        }
        else {
            hostVisualIndicador.SetActive(true);
            hostTMP.text = $"Player 1: {hostId}";
            clientVisualIndicador.SetActive(true);
            clientTMP.text = $"Player 2: {clientId}";
        }
    }

    void EachPlayerButtonsOn(bool isHost) {
        if (isHost) {
            changeCharacterOnButton.SetActive(true);
            readyButton[0].SetActive(true);
            playButton.SetActive(true);
        }
        else {
            changeCHaracterTwoButton.SetActive(true);
            readyButton[1].SetActive(true);
        }
    }

    private void OnClientDisconneted(ulong obj) {
        if (NetworkManager.Singleton.IsHost) {
            switch (NetworkManager.Singleton.ConnectedClients.Count) {
                case 0:
                    HostClearVisualClientRpc(true);
                    break;
                case 1:
                    HostClearVisualClientRpc(false);
                    break;
            }
        }
    }

    [ClientRpc]
    void HostClearVisualClientRpc(bool isHost) {
        if (isHost) {
            hostVisualIndicador.SetActive(false);
            hostTMP.text = "";

            clientVisualIndicador.SetActive(false);
            clientTMP.text = "";
        }
        else {
            clientVisualIndicador.SetActive(false);
            clientTMP.text = "";
        }
    }
    #endregion

    #region Buttons
    [ServerRpc(RequireOwnership = false)]
    public void ChangeCharacterButtonServerRpc(int player) {
        if (player == 1 && !WhiteBoard.Singleton.PlayerOneReady.Value) {
            WhiteBoard.Singleton.ChangeCharactersServerRpc(player);
        }
        else if (player == 2 && !WhiteBoard.Singleton.PlayerTwoReady.Value) {
            WhiteBoard.Singleton.ChangeCharactersServerRpc(player);
        }
    }
    public void ExitButton() {
        Application.Quit();
    }
    public void PlayButton() {
        if (WhiteBoard.Singleton.PlayerOneReady.Value && WhiteBoard.Singleton.PlayerTwoReady.Value) {
            StartCoroutine(LoadScene(nomeDaCena));
        }
        else {
            Debug.LogWarning("Jogadores não estão prontos");
        }
    }

    [ClientRpc]
    void LoadingScreenClientRpc() {
        loadingScreen.SetActive(true);
    }
    IEnumerator LoadScene(string sceneName) {
        LoadingScreenClientRpc();

        yield return new WaitForSecondsRealtime(loadingTime);

        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        yield return null;
    }

    public void BackButtonHostInLobby() {
        if (NetworkManager.Singleton.IsHost) {

            CloseLobbyClientRpc();

            NetworkManager.Singleton.Shutdown();
        }
    }

    [ClientRpc]
    void CloseLobbyClientRpc() {
        hostVisualIndicador.SetActive(false);
        hostTMP.text = "";

        clientVisualIndicador.SetActive(false);
        clientTMP.text = "";

        if (!NetworkManager.Singleton.IsHost) {
            lobbyScreen.SetActive(false);
            lobbyClosedScreen.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyButtonServerRpc(int player) {
        WhiteBoard.Singleton.CharacterReadyServerRpc(player);
    }

    #endregion
}
