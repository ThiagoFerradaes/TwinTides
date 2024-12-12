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
    ulong hostId;

    private void Start() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconneted;
        }
    }

    void OnDisable() {
        if (NetworkManager != null) {
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
    }

    [ClientRpc]
    void HostUpdateVisualsClientRpc(ulong clientId, bool isHost) {
        if (isHost) {
            hostId = clientId;
            hostVisualIndicador.SetActive(true);
            hostTMP.text = $"Player 1: {hostId}";
        }
        else {
            hostVisualIndicador.SetActive(true);
            hostTMP.text = $"Player 1: {hostId}";
            clientVisualIndicador.SetActive(true);
            clientTMP.text = $"Player 2: {clientId}";
        }

    }

    private void OnClientDisconneted(ulong obj) {
        HostClearVisualClientRpc(NetworkManager.Singleton.IsHost);
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

    public void ExitButton() {
        Application.Quit();
    }
    public void PlayButton() {
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
            StartCoroutine(LoadScene(nomeDaCena));
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

    #endregion
}
