using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuInGame : NetworkBehaviour {
    #region Variables
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Button pauseButton;
    [SerializeField] Button continueButton;

    [Header("Configuration Menu")]
    [SerializeField] GameObject configurationScreen;
    [SerializeField] Button configurationMenu;

    [Header("Leave menu")]
    [SerializeField] string menuSceneName;
    [SerializeField] float loadingTime;
    [SerializeField] GameObject loadingScreen, leavePopUp;
    [SerializeField] Button leaveButton, yesButtonPopUpLeave, noButtonPopUpLeave;

    readonly NetworkVariable<bool> _isPaused = new(false);
    #endregion

    #region Inicial Methods
    private void OnEnable() {
        _isPaused.OnValueChanged += IsPausedChanged; // Quando o valor do isPaused muda ele chama a função
        PlayerController.OnPause += ChangePauseState;

        NetworkManager.Singleton.OnServerStopped += ReturnToMenu;
    }

    private void OnDisable() {
        _isPaused.OnValueChanged -= IsPausedChanged;
        PlayerController.OnPause -= ChangePauseState;

        NetworkManager.Singleton.OnServerStopped -= ReturnToMenu;
    }
    private void Start() {
        SetButtons();
        UpdatePauseScreen(_isPaused.Value); // no começo só pra garantir 
    }
    #endregion

    #region Buttons
    void SetButtons() {
        // relacionados a pausar o jogo
        pauseButton.onClick.AddListener(ChangePauseState);
        continueButton.onClick.AddListener(ChangePauseState);

        // relacionados ao menu de configurações
        configurationMenu.onClick.AddListener(OpenConfigurations);

        // relacionados a sair do jogo e voltar para o menu
        leaveButton.onClick.AddListener(PopUpLeaveConfirmationOn);
        noButtonPopUpLeave.onClick.AddListener(PopUpLeaveConfirmationOff);
        yesButtonPopUpLeave.onClick.AddListener(LeaveGame);
    }
    void OpenConfigurations() {
        configurationScreen.SetActive(true);
    }
    #endregion

    #region Pause Order
    void ChangePauseState() {
        ChangePauseStateServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangePauseStateServerRpc() {
        _isPaused.Value = !_isPaused.Value;
    }
    void IsPausedChanged(bool oldPause, bool newPause) {
        UpdatePauseScreen(newPause);
    }
    void UpdatePauseScreen(bool pauseState) {
        if (pauseState) {
            Time.timeScale = 0f;
            ResetScreens();
            pauseScreen.SetActive(true);
        }
        else {
            Time.timeScale = 1f;
            TurnOffScreens();
        }
    }
    void ResetScreens() {
        configurationScreen.SetActive(false);
    }
    void TurnOffScreens() {
        configurationScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
    #endregion

    #region Leave 
    void PopUpLeaveConfirmationOn() {
        leavePopUp.SetActive(true);
    }
    void PopUpLeaveConfirmationOff() {
        leavePopUp.SetActive(false);
    }

    void LeaveGame() {
        ServerShutdownServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void ServerShutdownServerRpc() {
        NetworkManager.Singleton.Shutdown();
    }
    void ReturnToMenu(bool serverStopped) {
        Debug.Log("Server Stopped");
        StartCoroutine(ServerShutdown());
    }
    IEnumerator ServerShutdown() {
        loadingScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(loadingTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }
    #endregion
}
