using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : NetworkBehaviour {

    #region Inputs
    public void InputBaseAttack(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Ataque");
            UseSkillServerRpc(0);
        }
    }
    public void InputCommonRelicSkillOne(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Common Relic 1");
            UseSkillServerRpc(3);
        }
        
    }
    public void InputCommonRelicSkillTwo(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Common Relic 2");
            UseSkillServerRpc(4);
        }    
    }
    public void InputLegendarySkill(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Legendary Relic");
            UseSkillServerRpc(5);
        }  
    }
    #endregion

    [ServerRpc(RequireOwnership = false)] 
    void UseSkillServerRpc(int skillId) {
        UseSkillClientRPc(skillId);
    }

    [ClientRpc]
    void UseSkillClientRPc(int skillId) {

        SkillContext skillContext = new();

        switch (skillId) {
            case 0:
                if (LocalWhiteBoard.Instance.PlayerAttackSkill != null)
                    LocalWhiteBoard.Instance.PlayerAttackSkill.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 1:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null)
                    LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 2:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null)
                    LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 3:
                if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null)
                    LocalWhiteBoard.Instance.PlayerLegendarySkill.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
        }
    }
}
