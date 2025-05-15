using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : NetworkBehaviour {

    #region Variables

    [Header("Atributes")]
    [SerializeField] bool isSinglePlayer;

    [Header("Change scene")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] float loadingTime;
    [SerializeField] string nomeDaCena;

    [Header("Lobby")]
    [SerializeField] GameObject hostVisualIndicador;
    [SerializeField] GameObject clientVisualIndicador;
    [SerializeField] GameObject lobbyClosedScreen;
    [SerializeField] GameObject lobbyScreen;
    [SerializeField] Button changeCharacterOneButtonOne, changeCharacterOneButtonTwo, changeCharacterTwoButtonOne, changeCharacterTwoButtonTwo;
    [SerializeField] Button playButton;
    [SerializeField] Button[] readyButton;
    [SerializeField] Sprite readyButtonSprite, notReadyButtonSprite;
    [SerializeField] TextMeshProUGUI[] readyTexts;
    [SerializeField] GameObject joinByCodeScreen;
    [SerializeField] GameObject creatLobbyScreen;
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
    [SerializeField] GameObject failedToJoinLobbyScreen;
    [SerializeField] TextMeshProUGUI waitForHostOrJoinText;
    [SerializeField] float timeBetweenDots;
    private bool serverStarded;

    [Header("Joining screen")]
    [SerializeField] TMP_InputField[] inputFields;

    #endregion

    #region Methods

    private void Awake() {

        SetButtonsFunctions();
    }

    private void SetButtonsFunctions() {

        // botão de dar play
        playButton.onClick.AddListener(PlayButton);

        // botões para trocar de personagem
        changeCharacterOneButtonOne.onClick.AddListener(() => ChangeCharacterButtonServerRpc(1));
        changeCharacterOneButtonTwo.onClick.AddListener(() => ChangeCharacterButtonServerRpc(1));
        changeCharacterTwoButtonOne.onClick.AddListener(() => ChangeCharacterButtonServerRpc(2));
        changeCharacterTwoButtonOne.onClick.AddListener(() => ChangeCharacterButtonServerRpc(2));

        // botões ready
        readyButton[0].onClick.AddListener(() => ReadyButtonServerRpc(1));
        readyButton[1].onClick.AddListener(() => ReadyButtonServerRpc(2));
    }

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

    #endregion

    #region Funções De Entrada no Lobby
    private void OnClientConnected(ulong obj) { // Quando um client se conecta

        if (NetworkManager.Singleton.IsHost) { // Só o host chama essa função
            switch (NetworkManager.Singleton.ConnectedClients.Count) {
                case 0:
                    break;
                case 1:
                    HostUpdateVisualsClientRpc(obj, true); // Host se conectou
                    break;
                case 2:
                    HostUpdateVisualsClientRpc(obj, false); // Client se conectou
                    break;
            }
        }
        else {
            EachPlayerButtonsOn(false); // O client chama essa função quando ele entra
        }
    }

    [ClientRpc]
    void HostUpdateVisualsClientRpc(ulong clientId, bool isHost) {
        if (isHost) { // Ui que liga quando só o host entra na sal
            hostId = clientId;
            hostVisualIndicador.SetActive(true);
            EachPlayerButtonsOn(isHost);

            WhiteBoard.Singleton.ResetWhiteBoardServerRpc(); // Resetar o whiteBoard quando o host entra
        }
        else { // Ui que liga quando o client entra na sala
            hostVisualIndicador.SetActive(true);
            clientVisualIndicador.SetActive(true);
        }
    }

    void EachPlayerButtonsOn(bool isHost) {
        if (isHost) {
            changeCharacterTwoButtonOne.gameObject.SetActive(false); // Desligando os botões antigos caso o host tenha sido client
            changeCharacterTwoButtonTwo.gameObject.SetActive(false); // Desligando os botões antigos caso o host tenha sido client
            readyButton[1].gameObject.SetActive(false);

            changeCharacterOneButtonOne.gameObject.SetActive(true); // Ligando os botões certos
            changeCharacterOneButtonTwo.gameObject.SetActive(true); // Ligando os botões certos
            readyButton[0].gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
        }
        else {
            changeCharacterOneButtonOne.gameObject.SetActive(false); // Desligando os botões antigos caso o client tenha sido host
            changeCharacterOneButtonTwo.gameObject.SetActive(false); // Desligando os botões antigos caso o client tenha sido host
            readyButton[0].gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);


            changeCharacterTwoButtonOne.gameObject.SetActive(true); // Ligando os botões certos
            changeCharacterTwoButtonTwo.gameObject.SetActive(true); // Ligando os botões certos
            readyButton[1].gameObject.SetActive(true);
        }
    }

    private void OnClientDisconneted(ulong obj) { // Se o client desconecta limpa a ui dele
        clientVisualIndicador.SetActive(false);
    }
    #endregion

    #region Buttons
    [ServerRpc(RequireOwnership = false)]
    public void ChangeCharacterButtonServerRpc(int player) { // Botão de mudar de personagem
        if (player == 1 && !WhiteBoard.Singleton.PlayerOneReady.Value) {
            WhiteBoard.Singleton.ChangeCharactersServerRpc(player);
        }
        else if (player == 2 && !WhiteBoard.Singleton.PlayerTwoReady.Value) {
            WhiteBoard.Singleton.ChangeCharactersServerRpc(player);
        }
    }

    public void ExitButton() { // Botão de sair do jogo
        Application.Quit();
    }

    public void PlayButton() { // Botão de jogar

        // só pode trocar se os dois jogadores estiverem prontos ou for SinglePlayer
        if ((WhiteBoard.Singleton.PlayerOneReady.Value && WhiteBoard.Singleton.PlayerTwoReady.Value) || isSinglePlayer) { 
            LocalWhiteBoard.Instance.IsSinglePlayer = isSinglePlayer;
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
        loadingScreen.GetComponent<LoadingScreen>().Activate(loadingTime); 
        if (IsHost) {
            LocalWhiteBoard.Instance.PlayerCharacter = WhiteBoard.Singleton.PlayerOneCharacter.Value;
        }
        else {
            LocalWhiteBoard.Instance.PlayerCharacter = WhiteBoard.Singleton.PlayerTwoCharacter.Value;
        }
    }

    IEnumerator LoadScene(string sceneName) {
        LoadingScreenClientRpc();

        yield return new WaitForSecondsRealtime(loadingTime);

        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        yield return null;
    }

    public void BackButtonHostInLobby() { // Botão de sair do host
        if (NetworkManager.Singleton.IsHost) {
            CloseLobbyClientRpc();
        }
    }

    [ClientRpc]
    void CloseLobbyClientRpc() {

        hostVisualIndicador.SetActive(false);

        clientVisualIndicador.SetActive(false);

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
                readyButton[0].GetComponent<Image>().sprite = readyButtonSprite;
            }
            else { // Se vc n ta pronto, vc só pode dar pronto se o seu personagem for diferente do outro
                if (WhiteBoard.Singleton.PlayerOneCharacter.Value != WhiteBoard.Singleton.PlayerTwoCharacter.Value) {
                    WhiteBoard.Singleton.CharacterReadyServerRpc(player);
                    readyButton[0].GetComponent<Image>().sprite = notReadyButtonSprite;
                }
                else {
                    ShowDuplicatePopUpClientRpc(player);
                }
            }
        }
        if (player == 2) {
            if (WhiteBoard.Singleton.PlayerTwoReady.Value == true) { // Se vc ja ta pronto vc pode voltar
                WhiteBoard.Singleton.CharacterReadyServerRpc(player);
                readyButton[1].GetComponent<Image>().sprite = readyButtonSprite;
            }
            else { // Se vc n ta pronto, vc só pode dar pronto se o seu personagem for diferente do outro
                if (WhiteBoard.Singleton.PlayerOneCharacter.Value != WhiteBoard.Singleton.PlayerTwoCharacter.Value) {
                    WhiteBoard.Singleton.CharacterReadyServerRpc(player); ;
                    readyButton[1].GetComponent<Image>().sprite = notReadyButtonSprite;
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

    Coroutine _joinOrCreateLobbyCoroutine;
    public void HostOrJoinButton(bool host) {
        _joinOrCreateLobbyCoroutine = StartCoroutine(LoadingScreenCreatingLobbyOrJoin(host));
    }

    void ServerStarted() {
        serverStarded = true;
    }

    void ServerStopped(bool qualquer) {
        serverStarded = false;
    }

    IEnumerator LoadingScreenCreatingLobbyOrJoin(bool host) {

        if (host) {
            lobbyScreen.SetActive(true);
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

            creatLobbyScreen.SetActive(false);
            waitForHostOrJoinScreen.SetActive(false);

        }
        else {
            lobbyScreen.SetActive(true);
            waitForHostOrJoinScreen.SetActive(true);

            while (!NetworkManager.Singleton.IsConnectedClient) {
                waitForHostOrJoinText.text = "Joining lobby";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Joining lobby.";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Joining lobby..";
                yield return new WaitForSeconds(timeBetweenDots);
                waitForHostOrJoinText.text = "Joining lobby...";
                yield return new WaitForSeconds(timeBetweenDots);
            }

            joinByCodeScreen.SetActive(false);
            waitForHostOrJoinScreen.SetActive(false);
        }
    }
    public void FailedToJoinOrToCreate() {
        StopCoroutine(_joinOrCreateLobbyCoroutine); ;
    }

    public void CleanInputTextArea() {
        foreach (var item in inputFields) {
            item.text = "";
        }
    }

    #endregion
}
