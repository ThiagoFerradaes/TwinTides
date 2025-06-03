using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class HullbreakerEarthquake : SkillObjectPrefab {
    Hullbreaker _info;
    SkillContext _context;
    GameObject _maevis;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Hullbreaker;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefinePosition();
    }

    private void DefinePosition() {
        transform.localScale = new(_info.EarthquakeRadius, transform.localScale.y, _info.EarthquakeRadius);

        _context.Pos.y = GetGroundHeight(_context.Pos);

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.EarthquakeSound.IsNull) RuntimeManager.PlayOneShot(_info.EarthquakeSound);

        StartCoroutine(Duration());

    }
    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y;
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.EarthquakeDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.EarthquakeDamage);

        health.DealDamage(damage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
