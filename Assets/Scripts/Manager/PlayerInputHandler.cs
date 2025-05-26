using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

//public class PlayerInputHandler : NetworkBehaviour
//{
//    private PlayerMovement movement;
//    private PlayerDash dash;
//    private PlayerAim aim;
//    private PlayerInteraction interaction;
//    private PlayerSkillManager skillManager;

//    void Awake() {
//        movement = GetComponent<PlayerMovement>();
//        dash = GetComponent<PlayerDash>();
//        aim = GetComponent<PlayerAim>();
//        interaction = GetComponent<PlayerInteraction>();
//        skillManager = GetComponent<PlayerSkillManager>();
//    }

//    public void OnMove(InputAction.CallbackContext ctx) {
//        if (!CanProcessInput(ctx)) return;
//        movement.ProcessInput(ctx);
//    }

//    public void OnDash(InputAction.CallbackContext ctx) {
//        if (!CanProcessInput(ctx)) return;
//        dash.TryDash(ctx);
//    }

//    public void OnRotate(InputAction.CallbackContext ctx) {
//        if (!CanProcessInput(ctx)) return;
//        aim.Rotate(ctx);
//    }

//    public void OnInteract(InputAction.CallbackContext ctx) {
//        if (!CanProcessInput(ctx)) return;
//        interaction.TryInteract(ctx);
//    }
//    public void OnNormalAttack(InputAction.CallbackContext ctx) {
//        if (ctx.phase == InputActionPhase.Performed)
//            skillManager.TryUseSkill(0);
//    }

//    public void OnSkillOne(InputAction.CallbackContext ctx) {
//        if (ctx.phase == InputActionPhase.Performed)
//            skillManager.TryUseSkill(1);
//    }

//    public void OnSkillTwo(InputAction.CallbackContext ctx) {
//        if (ctx.phase == InputActionPhase.Performed)
//            skillManager.TryUseSkill(2);
//    }

//    public void OnLegendary(InputAction.CallbackContext ctx) {
//        if (ctx.phase == InputActionPhase.Performed)
//            skillManager.TryUseSkill(3);
//    }

//    bool CanProcessInput(InputAction.CallbackContext ctx) {
//        return IsOwner && Time.timeScale == 1 && !LocalWhiteBoard.Instance.AnimationOn;
//    }
//}
