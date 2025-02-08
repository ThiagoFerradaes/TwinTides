using Unity.Netcode;
using UnityEngine;

public abstract class SkillObjectPrefab : NetworkBehaviour {

    public string Name = "Bruno";

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOnSkillRpc(int skillId, int skillLevel, SkillContext context) {
        Debug.Log("TurnOnSKillRpc");

        Skill skill = PlayerSkillConverter.Instance.TransformIdInSkill(skillId);

        ActivateSkill(skill, skillLevel, context);
    }
    public abstract void ActivateSkill(Skill info, int skillLevel, SkillContext context);

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOffSkillRpc() {
        gameObject.SetActive(false);
    }

    public void Test(int skillId, int skillLevel, SkillContext context) {
        //TurnOnSkillRpc(skillId, skillLevel, context);
        TestRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void TestRpc() {
        gameObject.SetActive(true);
        Debug.Log("TestRpc");
        Invoke(nameof(TurnObjectOff), 2f);
    }

    void TurnObjectOff() {
        ReturnObjectRpc();
        //gameObject.SetActive(false);
    }

    [Rpc(SendTo.Server)]
    void ReturnObjectRpc() {
        PlayerSkillPooling.Instance.ReturnObjectToPool(this.gameObject);
    }
}
