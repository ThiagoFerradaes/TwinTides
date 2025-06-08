using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckPointsManager : NetworkBehaviour {
    public static CheckPointsManager Instance;

    [SerializeField] List<Totem> listOfTotensInGame;
    [SerializeField] Vector3 transformOffSett;
    Totem lastTotem;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(this);
    }

    public void RegisterLastTotem(Totem newTotem) {
        lastTotem = newTotem;
    }

    public Vector3 ReturnLastTotemPosition() {
        Vector3 lastTotemPosition;
        if (lastTotem != null) lastTotemPosition = lastTotem.transform.position + transformOffSett;
        else lastTotemPosition = new Vector3(0, 10, 0);
        return lastTotemPosition;
    }
}
