using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste_Dois : NetworkBehaviour {
    [SerializeField] float damage;
    [SerializeField] Debuff poison;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {

            HealthManager playerHealth = other.gameObject.GetComponent<HealthManager>();

            if (other.TryGetComponent<NetworkObject>(out NetworkObject obj)) {
                if (!obj.IsOwner) return;
                playerHealth.ApplyDamageOnServerRPC(damage, true, true);
                playerHealth.AddDebuffToList(poison as HealthDebuff);
            }
        }
    }

}
