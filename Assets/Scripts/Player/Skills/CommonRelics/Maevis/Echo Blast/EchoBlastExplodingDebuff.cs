using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EchoBlastExplodingDebuff : SkillObjectPrefab {
    EchoBlast _info;
    bool _canExplode, _canSetUpExplosion, _isPositioned;
    int _level;
    SkillContext _context;
    GameObject _maevis, _parent;

    List<HealthManager> events = new();

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        EchoBlastStunExplosion.OnExploded += EchoBlastStunExplosion_OnExploded;
    }
    private void EchoBlastStunExplosion_OnExploded(object sender, EchoBlastStunExplosion.ExplodedObject e) {
        DefineParent(e.target);
    }

    private void DefineParent(GameObject parent) {
        if (_isPositioned || ParentAlreadyHasDebuff(parent)) return;

        _isPositioned = true;

        _parent = parent;

        if (IsServer) {
            transform.SetParent(_parent.transform);

            transform.SetLocalPositionAndRotation(new Vector3(0, _info.ExplodingDebuffHeight, 0), Quaternion.Euler(0, 0, 0));
        }

        TurnObjectOnRpc();

        parent.TryGetComponent<HealthManager>(out HealthManager health);

        if (!events.Contains(health)) {
            health.OnGeneralDamage += Health_OnGeneralDamage;

            health.OnDeath += Health_OnDeath;

            events.Add(health);
        }

        StartCoroutine(DebuffDuration());

        StartCoroutine(WaitToStartExploding());
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TurnObjectOnRpc() {
        gameObject.SetActive(true);
    }

    private bool ParentAlreadyHasDebuff(GameObject parent) {
        foreach (Transform child in parent.transform) {
            if (child.GetComponent<EchoBlastExplodingDebuff>() != null) {
                return true; 
            }
        }
        return false;
    }

    private void Health_OnDeath() {
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

    private void Health_OnGeneralDamage(object sender, EventArgs e) {
        CanExplode();
    }

    void CanExplode() {
        if (_canSetUpExplosion) _canExplode = true;
    }

    IEnumerator Explode() {
        while (true && IsServer) {
            if (_canExplode && _parent != null) {
                _canExplode = false;
                _canSetUpExplosion = false;
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                SkillContext newContext = new(_parent.transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 3);

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

        EchoBlastStunExplosion.OnExploded -= EchoBlastStunExplosion_OnExploded;

        foreach(var enemie in events) {
            enemie.OnGeneralDamage -= Health_OnGeneralDamage;
            enemie.OnDeath -= Health_OnDeath;
        }

        events.Clear();

        if (IsServer) transform.parent = null;
        
        _isPositioned = false;
        
        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
