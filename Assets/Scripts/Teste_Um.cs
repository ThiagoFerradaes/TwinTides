using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Teste_Um : MonoBehaviour
{
    private HealthManager _healthManager;

    public HealthDebuff burnDebuff;
    public HealthDebuff posion;
    public HealthDebuff bleed;
    public HealthDebuff reducedHeal;
    public HealthDebuff reducedShield;
    public float healAmount;
    public float shieldAmount;


    private void Start() {
        _healthManager = GetComponent<HealthManager>();
    }

    private void Update() {

        if (Keyboard.current.lKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(burnDebuff);
        }
        if (Keyboard.current.kKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(posion);
        }
        if (Keyboard.current.oKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(bleed);
        }
        if (Keyboard.current.nKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(reducedHeal);
        }
        if (Keyboard.current.iKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(reducedShield);
        }
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            _healthManager.HealServerRpc(healAmount);
        }
        if (Keyboard.current.mKey.wasPressedThisFrame) {
            _healthManager.ReceiveShieldServerRpc(shieldAmount, 20, false);
        }
    }
}
