using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using Unity.Services.Vivox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Pop-Ups")]
    [SerializeField] Image notAllPlayersAreReadyPopUp;
    [SerializeField] Image duplicatedCharactersPopUP;
    [SerializeField] float timeToPopUpFadeOut;
    [SerializeField] float waitTimeToPopUpFadeOut;
    [Range(0f, 255f)][SerializeField] float popUpMaxAlpha;
    bool popUpCoroutinePlaying;

    [Header("Tela de carregamento pra host e join")]
    [SerializeField] GameObject waitForHostOrJoinScreen;
    [SerializeField] TextMeshProUGUI waitForHostOrJoinText;
    [SerializeField] float timeBetweenDots;
    private bool serverStarded;

    [Header("Joining screen")]
    [SerializeField] TMP_InputField[] inputFields;

    private void Start() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconneted;

            NetworkManager.Singleton.OnServerStarted += ServerStarted;
            NetworkManager.Singleton.OnServerStopped += ServerStopped;
        }

    }

    void OnDisable() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconneted;

            NetworkManager.Singleton.OnServerStarted -= ServerStarted;
            NetworkManager.Singleton.OnServerStopped -= ServerStopped;
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
            if (NetworkManager.Singleton.LocalClientId != obj) { // client saiu
                HostClearVisualClientRpc();
            }
        }
    }

    [ClientRpc]
    void HostClearVisualClientRpc() {
        clientVisualIndicador.SetActive(false);
        clientTMP.text = "";
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
            if (!popUpCoroutinePlaying) {
                StartCoroutine(PopUpFadeInFadeOut(notAllPlayersAreReadyPopUp));
            }
        }
    }

    IEnumerator PopUpFadeInFadeOut(Image popUp) {
        popUpCoroutinePlaying = true;

        Color original = popUp.color;
        popUp.color = new Color(original.r, original.g, original.b, 0f);

        TextMeshProUGUI textComponent = popUp.GetComponentInChildren<TextMeshProUGUI>();
        Color originalText = textComponent.color;
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0f);

        popUp.gameObject.SetActive(true);

        float maxAlpha = popUpMaxAlpha / 255f;


        for (float i = 0; i <= timeToPopUpFadeOut; i += Time.deltaTime) {
            float alpha = Mathf.Lerp(0, maxAlpha, i / timeToPopUpFadeOut);
            float textAlpha = Mathf.Lerp(0, 1, i / timeToPopUpFadeOut);

            popUp.color = new Color(original.r, original.g, original.b, alpha);
            textComponent.color = new Color(originalText.r, originalText.g, originalText.b, textAlpha);

            yield return null;
        }


        yield return new WaitForSeconds(waitTimeToPopUpFadeOut);


        for (float i = 0; i <= timeToPopUpFadeOut; i += Time.deltaTime) {
            float alpha = Mathf.Lerp(maxAlpha, 0, i / timeToPopUpFadeOut);
            float textAlpha = Mathf.Lerp(1, 0, i / timeToPopUpFadeOut);

            popUp.color = new Color(original.r, original.g, original.b, alpha);
            textComponent.color = new Color(originalText.r, originalText.g, originalText.b, textAlpha);

            yield return null;
        }

        popUp.gameObject.SetActive(false);
        popUp.color = original;
        textComponent.color = originalText;

        yield return new WaitForSeconds(0.2f);
        popUpCoroutinePlaying = false;
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

            playButton.SetActive(false);
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
        if (player == 1) {
            if (WhiteBoard.Singleton.PlayerOneReady.Value == true) { // Se vc ja ta pronto vc pode voltar
                WhiteBoard.Singleton.CharacterReadyServerRpc(player);
            }
            else { // Se vc n ta pronto, vc só pode dar pronto se o seu personagem for diferente do outro
                if (WhiteBoard.Singleton.PlayerOneCharacter.Value != WhiteBoard.Singleton.PlayerTwoCharacter.Value) {
                    WhiteBoard.Singleton.CharacterReadyServerRpc(player);
                }
                else {
                    ShowDuplicatePopUpClientRpc(player);
                }
            }
        }
        if (player == 2) {
            if (WhiteBoard.Singleton.PlayerTwoReady.Value == true) { // Se vc ja ta pronto vc pode voltar
                WhiteBoard.Singleton.CharacterReadyServerRpc(player);
            }
            else { // Se vc n ta pronto, vc só pode dar pronto se o seu personagem for diferente do outro
                if (WhiteBoard.Singleton.PlayerOneCharacter.Value != WhiteBoard.Singleton.PlayerTwoCharacter.Value) {
                    WhiteBoard.Singleton.CharacterReadyServerRpc(player); ;
                }
                else {
                    ShowDuplicatePopUpClientRpc(player);
                }
            }
        }
    }

    [ClientRpc]
    void ShowDuplicatePopUpClientRpc(int player) {
        if (player == 1) {
            if (NetworkManager.Singleton.IsHost)
                StartCoroutine(PopUpFadeInFadeOut(duplicatedCharactersPopUP));
        }
        else {
            if (!NetworkManager.Singleton.IsHost)
                StartCoroutine(PopUpFadeInFadeOut(duplicatedCharactersPopUP));
        }
    }

    public void HostOrJoinButton(bool host) {
        StartCoroutine(LoadingScreenCreatingLobbyOrJoin(host));
    }

    void ServerStarted() {
        serverStarded = true;
    }
    void ServerStopped(bool qualquer) {
        serverStarded = false;
    }
    IEnumerator LoadingScreenCreatingLobbyOrJoin(bool host) {
        if (host) {
            waitForHostOrJoinScreen.SetActive(true);

            while (!serverStarded) {
                waitForHostOrJoinText.text = "Creating lobby";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Creating lobby.";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Creating lobby..";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Creating lobby...";
                yield return new WaitForSeconds(timeBetweenDots);
            }

            waitForHostOrJoinScreen.SetActive(false);
        }
        else {
            waitForHostOrJoinScreen.SetActive(true);

            while (NetworkManager.Singleton.ConnectedClientsList.Count < 2) {
                waitForHostOrJoinText.text = "Joining lobby";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Joining lobby.";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Joining lobby..";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Joining lobby...";
                yield return new WaitForSeconds(timeBetweenDots);
            }

            waitForHostOrJoinScreen.SetActive(false);
        }
    }

    public void CleanInputTextArea() {
        foreach (var item in inputFields)
        {
            item.text = "";
        }
    }
    #endregion
}
