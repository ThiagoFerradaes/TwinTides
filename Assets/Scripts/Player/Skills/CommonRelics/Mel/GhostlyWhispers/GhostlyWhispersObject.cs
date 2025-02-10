using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostlyWhispersObject : SkillObjectPrefab {
    GhostlyWhispers _info;
    int _level;
    SkillContext _context;

    int _areaLevel = 0; // 0 = normal, 1 = super, 2 = mega
    bool _canDealDamage = true;

    MeshRenderer _mesh;
    [HideInInspector] public List<GhostlyWhispersObject> ActiveSkills;
    private void Awake() {
        _mesh = GetComponent<MeshRenderer>();
    }


    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as GhostlyWhispers;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPlace();
    }

    private void DefineSizeAndPlace() {
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.PlayerPosition + (direction * _info.PositionOffSet);
        transform.SetPositionAndRotation(position, _context.PlayerRotation);

        if (_level < 4) {
            transform.localScale = _info.Area;
        }
        else {
            transform.localScale = _info.AreaLevel4;
        }
        gameObject.SetActive(true);

        StartCoroutine(AreaDuration());
    }

    IEnumerator AreaDuration() {
        StartCoroutine(CheckDamageTimer());
        if (_level < 3) {
            yield return new WaitForSeconds(_info.AreaDuration);
        }
        else if (_level < 4) {
            yield return new WaitForSeconds(_info.AreaDurationLevel3);
        }
        else {
            yield return new WaitForSeconds(_info.AreaDurationLevel4);
        }

        ActiveSkills.Clear();
        _areaLevel = 0;
        _mesh.material = _info.NormalAreaMaterial;
        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<GhostlyWhispersObject>(out GhostlyWhispersObject skill)) return;

        if (!ActiveSkills.Contains(this)) ActiveSkills.Add(this);
        if (!ActiveSkills.Contains(skill)) ActiveSkills.Add(skill);

        skill.ActiveSkills = ActiveSkills;

        foreach (var g in ActiveSkills) g.RestartDuration();

        UpAreaLevel();

    }

    void UpAreaLevel() {
        if (_level == 3) {
            _areaLevel = 1;
            _mesh.material = _info.SuperAreaMaterial;
        }
        else if (_level == 4) {
            _areaLevel = 2;
            _mesh.material = _info.MegaAreaMaterial;
        }
    }
    public void RestartDuration() {
        UpAreaLevel();
        StopAllCoroutines();
        StartCoroutine(AreaDuration());
    }

    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Enemy") || !_canDealDamage) return;

        if (other.TryGetComponent<HealthManager>(out HealthManager health)) {
            if (_level < 2) {
                health.ApplyDamageOnServerRPC(_info.Damage, true, true);
            }
            else {
                health.ApplyDamageOnServerRPC(_info.DamageLevel2, true, true);
            }
        }
    }

    IEnumerator CheckDamageTimer() {
        _canDealDamage = true;
        if (_areaLevel == 0) {
            yield return new WaitForSeconds(_info.DamageInterval);
        }
        else if (_areaLevel == 1) {
            yield return new WaitForSeconds(_info.DamageIntervalLevel3);
        }
        else {
            yield return new WaitForSeconds(_info.DamageIntervalLevel4);
        }
        _canDealDamage = true;
        StartCoroutine(CheckDamageTimer());
    }
}
