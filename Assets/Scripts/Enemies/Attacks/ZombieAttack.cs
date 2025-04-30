using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ZombieAttack : EnemyAttack {

    [SerializeField] float timeBetweenPunches;
    [SerializeField] float damageOfEachPunch;
    [SerializeField] int comboNumberToUpgradeAttack;

    int skillId;
    Context parentContext;
    bool isServer;
    public override void StartAttack(Context parent) {
        skillId = EnemySkillConverter.Instance.TransformSkillInInt(this);
        parentContext = parent;
        isServer = NetworkManager.Singleton.IsServer;

        state = AttackState.RUNNING;

        if (parent.Blackboard.CurrentComboIndex < comboNumberToUpgradeAttack)
            parent.GameObject.GetComponent<MonoBehaviour>().StartCoroutine(PunchRoutine());
        else parent.GameObject.GetComponent<MonoBehaviour>().StartCoroutine(BetterPunchRoutine());
    }

    IEnumerator PunchRoutine() {
        if (isServer) EnemySkillPooling.Instance.RequestInstantiateAttakcRpc(skillId, 0, parentContext);

        yield return new WaitForSeconds(timeBetweenPunches);

        if (isServer) EnemySkillPooling.Instance.RequestInstantiateAttakcRpc(skillId, 0, parentContext);

        state = AttackState.SUCCESS;
        parentContext.Blackboard.CurrentComboIndex++;
    }

    IEnumerator BetterPunchRoutine() {
        if (isServer) EnemySkillPooling.Instance.RequestInstantiateAttakcRpc(skillId, 1, parentContext);

        yield return null;

        state = AttackState.SUCCESS;
        parentContext.Blackboard.CurrentComboIndex = 0;

        Debug.Log("BetterPunch");
    }
}
