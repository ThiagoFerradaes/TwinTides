using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Teste_Um : MonoBehaviour
{
    public CommonRelic soulSphere;

    private void Update() {
        if (!Keyboard.current.mKey.wasPressedThisFrame) return;
        if (!LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne == soulSphere) {
            LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne = soulSphere;
            LocalWhiteBoard.Instance.AddToCommonDictionary(soulSphere);
        }
        else {
            int newLevel = LocalWhiteBoard.Instance.CommonRelicInventory[soulSphere] + 1;
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(soulSphere, newLevel);
            Debug.Log("Up level to: " + newLevel); 
        }
    }
}
