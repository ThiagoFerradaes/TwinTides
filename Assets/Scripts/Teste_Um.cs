using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Teste_Um : MonoBehaviour {
    public AttackSkill attack;
    [SerializeField] Characters player = Characters.Mel;
    private void Start() {
        if (player == LocalWhiteBoard.Instance.PlayerCharacter) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = attack;
            LocalWhiteBoard.Instance.AttackLevel = 1;
        }
    }
    private void Update() {
        if (Keyboard.current.nKey.wasPressedThisFrame) {
            GetComponent<HealthManager>().DealDamage(1, false, false);
        }
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            GetComponent<HealthManager>().DealDamage(1, true, false);
        }

    }
}
