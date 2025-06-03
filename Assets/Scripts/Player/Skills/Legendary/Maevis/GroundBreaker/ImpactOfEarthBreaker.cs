using FMODUnity;
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

        _context.Pos.y = GetGroundHeight(_context.Pos);

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.EarthImpactSound.IsNull) RuntimeManager.PlayOneShot(_info.EarthImpactSound, transform.position);

        StartCoroutine(Duration());
    }

    Vector3 ReturnSize() {
        float x = _info.InicialImpactSize.x;
        for (int i = 0; i < _father._amountOfImpactsSummoned; i++) {
            x *= (1 + _info.ImpactGrowthPercent / 100); 
        }

        GameObject filho = transform.GetChild(0).gameObject;

        ParticleSystem ps = filho.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        var emission = ps.emission;

        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
        emission.GetBursts(bursts);

        bursts[0].count = new ParticleSystem.MinMaxCurve(x);
        emission.SetBursts(bursts);

        shape.scale = new Vector3(x, 0, 1);

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

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _dManager.ReturnTotalAttack(_info.Damage);

        if (health.ReturnShieldStatus()) health.BreakShield();

        health.DealDamage(damage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager mManager)) return;

        mManager.StunWithTime(_info.StunDuration);
    }

    void End() {
        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
