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
    }

    public void TurnScreenOn(CommonRelic relic, float gold) {
        if (relic != null) Debug.Log("Screen On: " + relic.Name + " Gold: " + gold);
        else Debug.Log("Screen On: Null Gold: " + gold);
    }
}
