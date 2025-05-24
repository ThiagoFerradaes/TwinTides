using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public override void OnNetworkSpawn() {
        gameObject.SetActive(false);
    }
}
