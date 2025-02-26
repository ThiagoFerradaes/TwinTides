using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CrimsonTidePath : SkillObjectPrefab {
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    List<HealthManager> _listOfEnemies = new();

    Coroutine _damageCoroutine;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }
    void SetPosition() {
        transform.localScale = _info.PathSize;

        _context.PlayerPosition.y = GetGroundHeight(_context.PlayerPosition);

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());

        _damageCoroutine = StartCoroutine(DamageCooldown());
    }

    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y;
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.PathDuration);

        End();
    }

    IEnumerator DamageCooldown() {
        float elapsedTime = 0f;
        while (elapsedTime < _info.PathDuration) {
            yield return new WaitForSeconds(_info.PathDamageInterval);
            elapsedTime += _info.PathDamageInterval;

            for (int i = _listOfEnemies.Count - 1; i >= 0; i--) {
                var health = _listOfEnemies[i];

                if (health == null) {
                    _listOfEnemies.RemoveAt(i);
                    continue;
                }

                bool shouldExecute = health.ReturnCurrentHealth() <= health.ReturnMaxHealth() * _info.PercentToExecute / 100 && _level >= 4;
                float damage = shouldExecute ? 9999 : _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);

                bool wasAlive = !health.ReturnDeathState();

                health.ApplyDamageOnServerRPC(damage, true, true);

                bool isDead = health.ReturnDeathState();

                if (_level == 4 && wasAlive && isDead) {
                    Health_OnDeathRpc();
                }
            }
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!_listOfEnemies.Contains(health)) _listOfEnemies.Add(health);

    }
    private void OnTriggerExit(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (_listOfEnemies.Contains(health)) _listOfEnemies.Remove(health);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void Health_OnDeathRpc() {
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter)
            _maevis.GetComponent<PlayerSkillManager>().ResetCooldown(_context.SkillIdInUI);
    }

    void End() {
        if (_damageCoroutine != null) {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }

        _listOfEnemies.Clear();

        ReturnObject();
    }
}
