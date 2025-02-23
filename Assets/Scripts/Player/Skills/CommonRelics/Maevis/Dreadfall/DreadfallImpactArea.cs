using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadfallImpactArea : SkillObjectPrefab {

    Dreadfall _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    List<HealthManager> _listOfEnemies = new();

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Dreadfall;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }

    private void SetPosition() {

        _context.PlayerPosition.y = GetGroundHeight(_context.PlayerPosition);

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());

        StartCoroutine(DamageCooldown());
    }

    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y;
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.FieldDuration);

        End();
    }

    IEnumerator DamageCooldown() {
        while (true) {
            yield return new WaitForSeconds(_info.DamageCooldown);
            float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.FieldDamagePerTick);

            foreach (var health in _listOfEnemies) {
                if (IsServer) health.ApplyDamageOnServerRPC(damage, true, true);

                health.AddDebuffToList(_info.BleedDebuff);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!_listOfEnemies.Contains(health)) _listOfEnemies.Add(health);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (_listOfEnemies.Contains(health)) _listOfEnemies.Remove(health);
    }

    void End() {
        _listOfEnemies.Clear();
        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
