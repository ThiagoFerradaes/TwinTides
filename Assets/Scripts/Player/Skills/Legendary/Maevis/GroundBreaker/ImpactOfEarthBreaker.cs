using System.Collections;
using UnityEngine;

public class ImpactOfEarthBreaker : SkillObjectPrefab {
    EarthBreaker _info;
    SkillContext _context;
    GameObject _maevis;
    DamageManager _dManager;
    EarthBreakerManager _father;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EarthBreaker;
        _context = context;
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
            _father = FindAnyObjectByType<EarthBreakerManager>();
        }

        SetPosition();
    }

    void SetPosition() {
        transform.localScale = ReturnSize();

        _context.PlayerPosition.y = GetGroundHeight(_context.PlayerPosition);

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    Vector3 ReturnSize() {
        float x = _info.InicialImpactSize.x;
        for (int i = 0; i < _father._amountOfImpactsSummoned; i++) {
            x *= (1 + _info.ImpactGrowthPercent / 100);
        }
        return new Vector3(x, _info.InicialImpactSize.y, _info.InicialImpactSize.z);
    }

    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y;
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ImpactDuration);
        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _dManager.ReturnTotalAttack(_info.Damage);

        if (health.ReturnShieldStatus()) health.BreakShieldRpc();

        health.ApplyDamageOnServerRPC(damage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager mManager)) return;

        mManager.StunWithTimeRpc(_info.StunDuration);
    }

    void End() {
        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
