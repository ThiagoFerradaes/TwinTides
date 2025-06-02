using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MenuSoundsManager : MonoBehaviour
{
    [SerializeField] EventReference hoverSound;
    [SerializeField] EventReference clickSound;
    [SerializeField] EventReference menuInitialMusic;
    [SerializeField] EventReference lobbySound;

    private EventInstance musicInstance;
    private EventReference currentMusic;

    private EventInstance ambienceInstance;

    private void Start() {
        PlayMusic(menuInitialMusic);
    }
    public void ClickSound() {
        RuntimeManager.PlayOneShot(clickSound);
    }

    public void HoverSound() {
        RuntimeManager.PlayOneShot(hoverSound);
    }

    public void PlayMusic(EventReference newMusic) {
        if (musicInstance.isValid()) {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }

        currentMusic = newMusic;
        musicInstance = RuntimeManager.CreateInstance(currentMusic);
        musicInstance.start();
    }

    public void StartLobbySound() {
        if (ambienceInstance.isValid()) {
            ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceInstance.release();
        }

        ambienceInstance = RuntimeManager.CreateInstance(lobbySound);
        ambienceInstance.start();
    }

    public void StopLobbySound() {
        if (ambienceInstance.isValid()) {
            ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceInstance.release();
        }
    }

    private void OnDestroy() {
        if (musicInstance.isValid()) {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
        StopLobbySound();
    }
}
