using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PauseMenuInGame : NetworkBehaviour {
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Button pauseButton;
    [SerializeField] Button continueButton;

    readonly NetworkVariable<bool> _isPaused = new(false);

    private void OnEnable() {
        _isPaused.OnValueChanged += IsPausedChanged; // Quando o valor do isPaused muda ele chama a fun��o
        PlayerController.OnPause += ChangePauseState;
    }
    private void OnDisable() {
        _isPaused.OnValueChanged -= IsPausedChanged;
        PlayerController.OnPause -= ChangePauseState;
    }
    private void Start() {
        pauseButton.onClick.AddListener(ChangePauseState); // Apertei o bot�o
        continueButton.onClick.AddListener(ChangePauseState);
        UpdatePauseScreen(_isPaused.Value); // no come�o s� pra garantir 
    }
    void IsPausedChanged(bool oldPause, bool newPause) {
        UpdatePauseScreen(newPause);
    }
    void UpdatePauseScreen(bool pauseState) {
        if (pauseState) {
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
        }
        else {
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
        }
    }
    void ChangePauseState() {
        ChangePauseStateServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangePauseStateServerRpc() {
        _isPaused.Value = !_isPaused.Value;
    }
}
