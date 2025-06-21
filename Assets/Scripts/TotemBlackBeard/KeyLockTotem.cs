using System;
using Unity.Netcode;
using UnityEngine;

public class KeyLockTotem : NetworkBehaviour
{
    [Header("Atributes")]
    [SerializeField] bool isMaevis;

    public static event Action<bool, bool> OnTurnScreenOn;
    public static event Action OnHasAllKeys;

    NetworkVariable<bool> hasAllKeys = new(false);

    #region Initialize
    private void Start() {
        KeyLockScreen.OnGiveAllKeys += KeyLockScreen_OnGiveAllKeys;
        hasAllKeys.OnValueChanged += OnHasAllKeysEvent;
    }
    public override void OnDestroy() {
        KeyLockScreen.OnGiveAllKeys -= KeyLockScreen_OnGiveAllKeys;
        hasAllKeys.OnValueChanged -= OnHasAllKeysEvent;
        base.OnDestroy();
    }
    #endregion

    void OnHasAllKeysEvent(bool oldBool, bool newBool) {
        OnHasAllKeys.Invoke();
    }
    private void KeyLockScreen_OnGiveAllKeys(bool isMaevis) {
        if (this.isMaevis == isMaevis) AllKyesUsedRpc();
    }

    [Rpc(SendTo.Server)]
    void AllKyesUsedRpc() {
        hasAllKeys.Value = true;
    }

    public void TurnScreenOn(object sender, System.EventArgs e) {
        OnTurnScreenOn?.Invoke(isMaevis, hasAllKeys.Value);
    }

    #region Trigger
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame += TurnScreenOn;
            controller.CanInteract = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController controller)) {
            controller.OnInteractInGame -= TurnScreenOn;
            controller.CanInteract = false;
        }
    }
    #endregion
}
