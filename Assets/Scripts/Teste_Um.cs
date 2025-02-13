using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Teste_Um : MonoBehaviour {
    public CommonRelic relic;
    public AttackSkill attack;
    private void Start() {
        LocalWhiteBoard.Instance.PlayerAttackSkill = attack;
        LocalWhiteBoard.Instance.AttackLevel = 1;
    }
    private void Update() {
        if (Keyboard.current.nKey.wasPressedThisFrame) {
            Debug.Log("1 dano");
            GetComponent<HealthManager>().ApplyDamageOnServerRPC(1, false, false);
        }

        if (!Keyboard.current.mKey.wasPressedThisFrame) return;
        if (!LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne == relic) {
            LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne = relic;
            LocalWhiteBoard.Instance.AddToCommonDictionary(relic);
            Debug.Log("Relic: " + LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.name);
        }
        else {
            int newLevel = LocalWhiteBoard.Instance.CommonRelicInventory[relic] + 1;
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(relic, newLevel);
            Debug.Log("Up level to: " + newLevel);
        }
    }
}
