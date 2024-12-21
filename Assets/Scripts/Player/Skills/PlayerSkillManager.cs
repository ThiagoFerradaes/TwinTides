using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : NetworkBehaviour {

    [SerializeField] bool teste;

    #region Inputs
    public void InputBaseAttack(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Ataque");
            UseSkillServerRpc(0);
        }
    }
    public void InputNpcSkillOne(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("NPC SKill One");
            UseSkillServerRpc(1);
        }
    }
    public void InputNpcSkillTwo(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("NPC SKill Two");
            UseSkillServerRpc(2);
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

    [ServerRpc(RequireOwnership = false)] // A partir daqui já não é mais um exemplo, já é o real
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
                if (LocalWhiteBoard.Instance.PlayerNpcSkillOne != null)
                    LocalWhiteBoard.Instance.PlayerNpcSkillOne.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 2:
                if (LocalWhiteBoard.Instance.PlayerNpcSkillTwo != null)
                    LocalWhiteBoard.Instance.PlayerNpcSkillTwo.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 3:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null)
                    LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 4:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null)
                    LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
            case 5:
                if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null)
                    LocalWhiteBoard.Instance.PlayerLegendarySkill.UseSkill(skillContext);
                else Debug.Log("No Skill");
                break;
        }
    }
}
