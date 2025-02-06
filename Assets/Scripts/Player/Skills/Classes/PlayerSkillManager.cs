using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : NetworkBehaviour {

    #region Inputs
    public void InputBaseAttack(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Ataque");
            UseSkill(0);
        }
    }
    public void InputCommonRelicSkillOne(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Common Relic 1");
            UseSkill(1);
        }
        
    }
    public void InputCommonRelicSkillTwo(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Common Relic 2");
            UseSkill(2);
        }    
    }
    public void InputLegendarySkill(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            Debug.Log("Legendary Relic");
            UseSkill(3);
        }  
    }
    #endregion

    void UseSkill(int skillId) {
        SkillContext skillContext = new(this.transform.position, this.transform.rotation);

        Debug.Log("UseSKill PlayerSkillManager");
        switch (skillId) {
            case 0:
                if (LocalWhiteBoard.Instance.PlayerAttackSkill != null) {
                    AttackSkill skill = LocalWhiteBoard.Instance.PlayerAttackSkill;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.AttackLevel);
                }
                else Debug.Log("No Skill");
                break;
            case 1:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
                    CommonRelic skill = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.CommonRelicInventory[skill]);
                }      
                else Debug.Log("No Skill");
                break;
            case 2:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null) {
                    CommonRelic skill = LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.CommonRelicInventory[skill]);
                }
                else Debug.Log("No Skill");
                break;
            case 3:
                if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null) {
                    LegendaryRelic skill = LocalWhiteBoard.Instance.PlayerLegendarySkill;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.LegendaryRelicInventory[skill]);
                }
                else Debug.Log("No Skill");
                break;
        }
    }
}
