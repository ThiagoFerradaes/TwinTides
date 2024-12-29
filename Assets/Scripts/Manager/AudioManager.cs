using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Slider generalSlider, musicSlider, sfxSlider, ambienceSlider, dialogueSlider;

    FMOD.Studio.Bus generalBus, musicBus, sfxBus, ambienceBus, dialogueBus;

    #region Métodos dos sliders
    public void ChangeGeneralVolume(float volume) {
        LocalWhiteBoard.Instance.GeneralVolume = volume;
        generalBus.setVolume(volume);
    }
    public void ChangeMusicVolume(float volume) {
        LocalWhiteBoard.Instance.MusicVolume = volume;
        musicBus.setVolume(volume);
    }
    public void ChangeSFXVolume(float volume) {
        LocalWhiteBoard.Instance.SFXVolume = volume;
        sfxBus.setVolume(volume);
    }
    public void ChangeAmbienceVolume(float volume) {
        LocalWhiteBoard.Instance.AmbienceVolume = volume;
        ambienceBus.setVolume(volume);
    }
    public void ChangeDialogueValue(float volume) {
        LocalWhiteBoard.Instance.DialogueVolume = volume;
        dialogueBus.setVolume(volume);
    }
    #endregion

    #region Métodos
    private void Start() {
        GetBus();
        SetSliders();
        SetBusVolume();
    }
    void GetBus() {
        generalBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogues");
    }
    void SetSliders() {
        generalSlider.value = LocalWhiteBoard.Instance.GeneralVolume;
        musicSlider.value = LocalWhiteBoard.Instance.MusicVolume;
        sfxSlider.value = LocalWhiteBoard.Instance.SFXVolume;
        ambienceSlider.value = LocalWhiteBoard.Instance.AmbienceVolume;
        dialogueSlider.value = LocalWhiteBoard.Instance.DialogueVolume;
    }
    void SetBusVolume() {
        generalBus.setVolume(LocalWhiteBoard.Instance.GeneralVolume);
        musicBus.setVolume(LocalWhiteBoard.Instance.MusicVolume);
        sfxBus.setVolume(LocalWhiteBoard.Instance.SFXVolume);
        ambienceBus.setVolume(LocalWhiteBoard.Instance.AmbienceVolume);
        dialogueBus.setVolume(LocalWhiteBoard.Instance.DialogueVolume);
    }
    #endregion
}
