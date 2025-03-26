using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Teste_Um : MonoBehaviour {
    public CommonRelic relic;
    public LegendaryRelic Lrelic;
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
            GetComponent<HealthManager>().ApplyDamageOnServerRPC(1, false, false);
        }
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            GetComponent<HealthManager>().ApplyDamageOnServerRPC(1, true, false);
        }

        if (!Keyboard.current.mKey.wasPressedThisFrame) return;
        if (!LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne == relic) {
            LocalWhiteBoard.Instance.AddToCommonDictionary(relic);
            LocalWhiteBoard.Instance.EquipRelic(relic, 1);
            Debug.Log("Relic: " + LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.name);

            LocalWhiteBoard.Instance.AddToLegendaryDictionary(Lrelic);
            LocalWhiteBoard.Instance.EquipRelic(Lrelic, 3);
            Debug.Log("Legendary Relic: " + LocalWhiteBoard.Instance.PlayerLegendarySkill.name);
        }
        else {
            int newLevel = LocalWhiteBoard.Instance.CommonRelicInventory[relic] + 1;
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(relic, newLevel);
            Debug.Log("Up level to: " + newLevel);
        }
    }
}
