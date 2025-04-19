using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GhostlyWhispersPuddle : SkillObjectPrefab {
    GhostlyWhispers _info;
    int _level;
    SkillContext _context;

    int _areaLevel = 0; // 0 = normal, 1 = super, 2 = mega

    GameObject _mel;

    MeshRenderer _mesh;
    [HideInInspector] public List<GhostlyWhispersPuddle> ActiveSkills;
    List<HealthManager> listOfEnemies = new();
    private void Awake() {
        _mesh = GetComponent<MeshRenderer>();
    }


    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as GhostlyWhispers;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefineSizeAndPlace();
    }

    private void DefineSizeAndPlace() {
        transform.localScale = _level < 4 ? _info.Area : _info.AreaLevel4; 

        _context.PlayerPosition.y = GetFloorHeight(_context.PlayerPosition);

        Transform aim = _mel.GetComponent<PlayerController>().aimObject;
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.PlayerPosition + (direction * _info.MaxRange);

        if (aim != null && aim.gameObject.activeInHierarchy && Vector3.Distance(_context.PlayerPosition, aim.position) <= _info.MaxRange) {
            transform.SetPositionAndRotation(new Vector3(aim.position.x, _context.PlayerPosition.y, aim.position.z), _context.PlayerRotation);
        }
        else {
            transform.SetPositionAndRotation(position, _context.PlayerRotation);
        }

        _areaLevel = _level < 4 ? 0 : 1;

        DefineMaterial();

        gameObject.SetActive(true);

        StartCoroutine(AreaDuration());
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
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

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<GhostlyWhispersPuddle>(out GhostlyWhispersPuddle skill)) {

            if (!ActiveSkills.Contains(this)) ActiveSkills.Add(this);
            if (!ActiveSkills.Contains(skill)) ActiveSkills.Add(skill);

            skill.ActiveSkills = ActiveSkills;

            foreach (var g in ActiveSkills) g.RestartDuration();

            UpAreaLevel();
        }

        if (!other.CompareTag("Enemy")) return;
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;
        if (!listOfEnemies.Contains(health)) listOfEnemies.Add(health);

    }

    void UpAreaLevel() {
        if (_level == 3) {
            _areaLevel = 1;
        }
        else if (_level == 4) {
            _areaLevel = 2;
        }
        DefineMaterial();
    }

    void DefineMaterial() {
        switch (_areaLevel) {
            case 0: _mesh.material = _info.NormalAreaMaterial; break;
            case 1: _mesh.material = _info.SuperAreaMaterial; break;
            case 2: _mesh.material = _info.MegaAreaMaterial; break;
        }
    }
    public void RestartDuration() {
        UpAreaLevel();
        StopAllCoroutines();
        StartCoroutine(AreaDuration());
    }

    IEnumerator CheckDamageTimer() {
        while (true) {

            float damageInterval = _areaLevel switch {
                0 => _info.DamageInterval,
                1 => _info.DamageIntervalLevel3,
                _ => _info.DamageIntervalLevel4
            };

            yield return new WaitForSeconds(damageInterval);

            float damage = _level == 1 ? _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.Damage) :
                _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.DamageLevel2);

            foreach (var enemie in listOfEnemies) {
                enemie.DealDamage(damage, true, true);
            }

            yield return null;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Enemy")) return;
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (listOfEnemies.Contains(health)) listOfEnemies.Remove(health); 
    }

    void End() {
        ActiveSkills.Clear();

        _areaLevel = 0;

        _mesh.material = _info.NormalAreaMaterial;

        listOfEnemies.Clear();

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
