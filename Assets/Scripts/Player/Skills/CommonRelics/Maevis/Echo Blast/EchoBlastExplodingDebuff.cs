using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoBlastExplodingDebuff : SkillObjectPrefab {
    EchoBlast _info;
    bool _canExplode, _canSetUpExplosion, _isPositioned;
    int _level;
    SkillContext _context;
    GameObject _maevis, _parent;
    float _currentEnemyHealth;

    List<HealthManager> _trackedHealthManagers = new();

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        _maevis = _maevis != null ? _maevis : PlayerSkillPooling.Instance.MaevisGameObject;

        EchoBlastStunExplosion.OnExploded += OnExplosionTriggered;
    }
    private void OnExplosionTriggered(object sender, EchoBlastStunExplosion.ExplodedObject e) {
        TryAttachToTarget(e.target);
    }

    private void TryAttachToTarget(GameObject parent) {
        if (_isPositioned || TargetAlreadyHasDebuff(parent)) return;

        _isPositioned = true;

        _parent = parent;

        transform.SetParent(_parent.transform);

        transform.SetLocalPositionAndRotation(new Vector3(0, _info.ExplodingDebuffHeight, 0), Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        if (parent.TryGetComponent(out HealthManager health) && !_trackedHealthManagers.Contains(health)) {
            _currentEnemyHealth = health.ReturnCurrentHealth();
            health.OnHealthUpdate += OnHealthChanged;
            health.OnDeath += OnDeath;
            _trackedHealthManagers.Add(health);
        }

        StartCoroutine(DebuffDuration());

        StartCoroutine(WaitToStartExploding());
    }


    private bool TargetAlreadyHasDebuff(GameObject parent) {
        foreach (Transform child in parent.transform) {
            if (child.GetComponent<EchoBlastExplodingDebuff>() != null) {
                return true;
            }
        }
        return false;
    }

    private void OnDeath() {
        End();
    }

    IEnumerator DebuffDuration() {
        StartCoroutine(Explode());
        yield return new WaitForSeconds(_info.ExplodingDebuffDuration);

        End();
    }
    IEnumerator WaitToStartExploding() {
        yield return new WaitForSeconds(_info.ExplodingDebuffDelay);

        _canSetUpExplosion = true;
    }

    private void OnHealthChanged((float maxHealth, float currentHealth, float currentShield)health) {
        if (_currentEnemyHealth > health.currentHealth) {
            if (_canSetUpExplosion) _canExplode = true;
            _currentEnemyHealth = health.currentHealth;
        }
    }


    IEnumerator Explode() {
        while (true) {
            if (_canExplode && _parent != null && LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
                _canExplode = false;
                _canSetUpExplosion = false;
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                SkillContext newContext = new(_parent.transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 3);

                StartCoroutine(ExplosionCooldown());
            }
            yield return null;
        }
    }

    IEnumerator ExplosionCooldown() {
        yield return new WaitForSeconds(_info.ExplodingDebuffExplosionCooldown);

        _canSetUpExplosion = true;
    }

    void End() {
        StopAllCoroutines();

        EchoBlastStunExplosion.OnExploded -= OnExplosionTriggered;

        foreach (var enemie in _trackedHealthManagers) {
            enemie.OnHealthUpdate -= OnHealthChanged;
            enemie.OnDeath -= OnDeath;
        }

        _trackedHealthManagers.Clear();

        transform.parent = null;

        _isPositioned = false;

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
