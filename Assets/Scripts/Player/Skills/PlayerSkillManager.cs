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
            _skillOne_Cooldown = LocalWhiteBoard.Instance.PlayerNpcSkillOne.Cooldown;
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
                LocalWhiteBoard.Instance.PlayerAttackSkill.UseSkill(skillContext);
                break;
            case 1:
                LocalWhiteBoard.Instance.PlayerNpcSkillOne.UseSkill(skillContext);
                break;
            case 2:
                LocalWhiteBoard.Instance.PlayerNpcSkillTwo.UseSkill(skillContext);
                break;
            case 3:
                LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.UseSkill(skillContext);
                break;
            case 4:
                LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo.UseSkill(skillContext);
                break;
            case 5:
                LocalWhiteBoard.Instance.PlayerLegendarySkill.UseSkill(skillContext);
                break;
        }
    }
}
