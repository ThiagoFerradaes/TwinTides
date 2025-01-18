using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PauseMenuInGame : NetworkBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Button pauseButton;

    readonly NetworkVariable<bool> _isPaused = new(false);

    private void OnEnable() {
        _isPaused.OnValueChanged += IsPausedChanged; // Quando o valor do isPaused muda ele chama a função
    }
    private void OnDisable() {
        _isPaused.OnValueChanged -= IsPausedChanged;
    }
    private void Start() {
        pauseButton.onClick.AddListener(ChangePauseState); // Apertei o botão
        UpdatePauseScreen(_isPaused.Value); // no começo só pra garantir 
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
        //ChangePauseStateServerRpc();
        Debug.Log("BOtao");
    }
    [ServerRpc (RequireOwnership = false)]
    public void ChangePauseStateServerRpc() {
        _isPaused.Value = !_isPaused.Value;
    }
}
