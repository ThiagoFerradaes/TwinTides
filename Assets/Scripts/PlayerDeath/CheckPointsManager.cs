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
        Vector3 basePosition = lastTotem != null ? lastTotem.transform.position + transformOffSett : new Vector3(0, 10, 0);

        if (Physics.Raycast(basePosition, Vector3.down, out RaycastHit hitInfo, 20f, LayerMask.GetMask("Floor"))) {
            float playerHalfHeight = 1f; 
            return hitInfo.point + Vector3.up * playerHalfHeight;
        }

        return basePosition;
    }
}
