using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GirlXTwoHealingArea : EnemyAttackPrefab
{
    GirlXTwo _info;
    HashSet<HealthManager> _listOfEnemies = new();
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlXTwo;

        parentContext.Blackboard.IsAttacking = true;

        SetPosition();
    }

    void SetPosition() {
        transform.position = parent.transform.position;

        transform.localScale = _info.healingSize * Vector3.one;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        StartCoroutine(HealingRoutine());

        yield return new WaitForSeconds(_info.healingDuration);

        End();
    }

    IEnumerator HealingRoutine() {
        while (true) {
            foreach(var enemy in _listOfEnemies) {
                if (enemy.ReturnCurrentHealth() < enemy.ReturnMaxHealth()) enemy.Heal(_info.amountOfHealing, false);
                else enemy.ApplyShield(_info.amountOfShield, _info.durationOfShield, true);

                if (_info is GirlTwoTwo) enemy.CleanAllDebuffs();
            }

            yield return new WaitForSeconds(_info.timeBetweenHeals);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfEnemies.Add(health);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfEnemies.Remove(health);
    }

    public override void End() {
        _listOfEnemies.Clear();

        parentContext.Blackboard.IsAttacking = false;
        parentContext.Blackboard.CanAttack = false;
        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.cooldown;

        base.End();
    }
}
