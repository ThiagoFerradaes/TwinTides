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
    public HealthDebuff shieldBlocker;
    public HealthDebuff healBlocker;
    public HealthBuff healOverTime;
    public HealthBuff debuffBlock;
    public HealthBuff invulnerable;
    public HealthBuff healIncrease;
    public HealthBuff damageDecreased;
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
            _healthManager.AddBuffToList(healOverTime);
        }
        if (Keyboard.current.mKey.wasPressedThisFrame) {
            _healthManager.ApplyShieldServerRpc(shieldAmount, 20, false);
        }
        if (Keyboard.current.jKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(shieldBlocker);
        }
        if (Keyboard.current.bKey.wasPressedThisFrame) {
            _healthManager.AddDebuffToList(healBlocker);
        }
        if (Keyboard.current.vKey.wasPressedThisFrame) {
            _healthManager.AddBuffToList(invulnerable);
        }
        if (Keyboard.current.uKey.wasPressedThisFrame) {
            _healthManager.AddBuffToList(debuffBlock);
        }
        if (Keyboard.current.xKey.wasPressedThisFrame) {
            _healthManager.AddBuffToList(damageDecreased);
        }
        if (Keyboard.current.zKey.wasPressedThisFrame) {
            _healthManager.AddBuffToList(healIncrease);
        }
    }
}
