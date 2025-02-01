using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Teste_Um : MonoBehaviour
{
    public CommonRelic soulSphere;

    private void Update() {
        if (Keyboard.current.mKey.isPressed) {
            LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne = soulSphere;
            LocalWhiteBoard.Instance.AddToCommonDictionary(soulSphere);
        }
    }
}
