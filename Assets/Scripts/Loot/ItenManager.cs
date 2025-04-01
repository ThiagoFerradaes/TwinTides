using System;
using UnityEngine;

public class ItenManager : MonoBehaviour
{
    public static ItenManager Instance;
    public GameObject ItensScreen;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    private void Start() {
        PlayerController.OnInteractOutGame += PlayerController_OnInteractOutGame;
    }

    private void PlayerController_OnInteractOutGame(object sender, System.EventArgs e) {
        TurnScreeenOff();
    }

    private void TurnScreeenOff() {
        Debug.Log("Screen Off");
        Time.timeScale = 1f;
        LocalWhiteBoard.Instance.AnimationOn = false;
    }

    public void TurnScreenOn() {
        Debug.Log("Screen On");
        Time.timeScale = 0f;
        LocalWhiteBoard.Instance.AnimationOn = true;
    }
}
