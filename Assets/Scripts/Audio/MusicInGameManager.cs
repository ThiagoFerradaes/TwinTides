using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum MusicState { Exploration, Combat, Boss }
public class MusicInGameManager : NetworkBehaviour {
    public static MusicInGameManager Instance;

    [Header("FMOD Events")]
    [SerializeField] private EventReference explorationEvent;
    [SerializeField] private EventReference combatEvent;
    [SerializeField] private EventReference bossEvent;

    private EventInstance currentMusic;
    private Coroutine musicLoopCoroutine;

    private MusicState currentState;
    private int currentTrackIndex;
    private bool isPlaying = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        SetMusicState(MusicState.Exploration);
    }

    public override void OnDestroy() {
        if (currentMusic.isValid()) {
            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
        }
        base.OnDestroy();
    }
    public void SetMusicState(MusicState newState) {
        if (!IsServer) return;
        if (currentState == newState && isPlaying) return;

        currentState = newState;
        StartNextMusicTrack();
    }

    private void StartNextMusicTrack() {
        if (musicLoopCoroutine != null) StopCoroutine(musicLoopCoroutine);

        currentTrackIndex = GetRandomTrackIndex(currentState);
        PlayMusicRpc(currentState, currentTrackIndex);
        musicLoopCoroutine = StartCoroutine(MusicLoopCoroutine());
    }

    private int GetRandomTrackIndex(MusicState state) {
        return state switch {
            MusicState.Exploration => Random.Range(0, 3),
            MusicState.Combat => Random.Range(0, 3),
            MusicState.Boss => 0, // Só tem uma música
            _ => 0
        };
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayMusicRpc(MusicState state, int trackIndex) {
        // Libera música anterior
        if (currentMusic.isValid()) {
            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
        }

        EventReference musicRef = GetEventReferenceForState(state);
        currentMusic = RuntimeManager.CreateInstance(musicRef);

        // Define o parâmetro da faixa (exceto boss que só tem 1)
        if (state != MusicState.Boss) currentMusic.setParameterByName("TrackIndex", trackIndex);

        currentMusic.start();
        isPlaying = true;
    }

    private IEnumerator MusicLoopCoroutine() {
        yield return new WaitForSeconds(0.5f); // tempo pro start() funcionar

        if (!currentMusic.isValid()) yield break;

        currentMusic.getDescription(out EventDescription desc);
        desc.getLength(out int lengthMs);

        yield return new WaitForSeconds(lengthMs / 1000f); // espera a faixa acabar

        StartNextMusicTrack(); // recomeça com nova faixa aleatória
    }

    private EventReference GetEventReferenceForState(MusicState state) {
        return state switch {
            MusicState.Exploration => explorationEvent,
            MusicState.Combat => combatEvent,
            MusicState.Boss => bossEvent,
            _ => explorationEvent
        };
    }
}
