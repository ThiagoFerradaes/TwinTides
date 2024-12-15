using Unity.Netcode;
using UnityEngine;

public class PlayerSkillManager : NetworkBehaviour {

    float _skillOne_Cooldown;

    private void Update() {
        _skillOne_Cooldown = Time.time;
    }
    public void UseSkill() {
        if (_skillOne_Cooldown <= 0) {
            UseSkillServerRpc(1); // ISSO AQUI FOI SÓ UM EXEMPLO
            _skillOne_Cooldown = LocalWhiteBoard.PLAYER_NPC_SKILL_ONE.Cooldown;
        }
    }

    [ServerRpc(RequireOwnership = false)] // A partir daqui já não é mais um exemplo, já é o real
    void UseSkillServerRpc(int skillId) {
        UseSkillClientRPc(skillId);
    }

    [ClientRpc]
    void UseSkillClientRPc(int skillId) {

        SkillContext skillContext = new();

        switch (skillId) {
            case 0:
                LocalWhiteBoard.PLAYER_ATTACK_SKILL.UseSkill(skillContext);
                break;
            case 1:
                LocalWhiteBoard.PLAYER_NPC_SKILL_ONE.UseSkill(skillContext);
                break;
            case 2:
                LocalWhiteBoard.PLAYER_NPC_SKILL_TWO.UseSkill(skillContext);
                break;
            case 3:
                LocalWhiteBoard.PLAYER_COMMON_RELIC_SKILL_ONE.UseSkill(skillContext);
                break;
            case 4:
                LocalWhiteBoard.PLAYER_COMMON_RELIC_SKILL_TWO.UseSkill(skillContext);
                break;
            case 5:
                LocalWhiteBoard.PLAYER_LEGENDARY_SKILL.UseSkill(skillContext);
                break;
        }
    }
}
