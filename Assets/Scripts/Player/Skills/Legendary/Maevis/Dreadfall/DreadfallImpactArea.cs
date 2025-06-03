using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadfallImpactArea : SkillObjectPrefab {

    Dreadfall _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    DamageManager _dManager;

    List<HealthManager> _listOfEnemies = new();

    EventInstance soundInstance;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Dreadfall;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
        }

        SetPosition();
    }

    private void SetPosition() {
        transform.localScale = new(_info.FieldRadius, transform.localScale.y, _info.FieldRadius);

        _context.Pos.y = GetGroundHeight(_context.Pos);

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.BurningAreaSound.IsNull) {
            soundInstance = RuntimeManager.CreateInstance(_info.BurningAreaSound);
            RuntimeManager.AttachInstanceToGameObject(soundInstance, this.gameObject);
            soundInstance.start();
        }

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

        if (!_info.BurningAreaSound.IsNull) {
            soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            soundInstance.release();
        }

        End();
    }

    IEnumerator DamageCooldown() {
        while (true) {
            yield return new WaitForSeconds(_info.DamageCooldown);
            float damage = _dManager.ReturnTotalAttack(_info.FieldDamagePerTick);

            foreach (var health in _listOfEnemies) {
                health.DealDamage(damage, true, true);

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
