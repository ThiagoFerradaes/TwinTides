using System;
using System.Collections;
using UnityEngine;

public class SpectralSeedsObject : SkillObjectPrefab {

    SpectralSeeds _info;
    int _level;
    SkillContext _contex;
    SpectralSeedsRing _father;

    public static event EventHandler OnSphereMoved;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {

        _info = info as SpectralSeeds;
        _level = skillLevel;
        _contex = context;

        TurnSeedOn();
    }

    void TurnSeedOn() {
        if (_father == null) {
            _father = GameObject.FindAnyObjectByType<SpectralSeedsRing>();
        }

        transform.localScale = _info.SeedSize;

        _father.listOfSeeds.Add(this);

        transform.SetParent(_father.transform);

        transform.SetLocalPositionAndRotation(GetPosition(), _contex.PlayerRotation);

        gameObject.SetActive(true);

        MelNormalAttackObject.OnNormalAttack += MelNormalAttackObject_OnNormalAttack;
    }

    private void MelNormalAttackObject_OnNormalAttack(object sender, MelNormalAttackObject.NormalAtackEventArgs e) {
        if (!gameObject.activeSelf) return;
        if (_father.listOfSeeds.IndexOf(this) == 0) {
            StartCoroutine(Move(e.FinalPosition));
            StartCoroutine(WaitForUpdateNextSpeed());
        }
    }

    IEnumerator WaitForUpdateNextSpeed() {
        yield return null;
        _father.listOfSeeds.Remove(this);
        OnSphereMoved?.Invoke(this, EventArgs.Empty);
    }

    Vector3 GetPosition() {
        float radius = _info.SeedRadius;
        float amountOfSeeds = _level switch {
            1 => _info.AmountOfSeeds,
            2 => _info.AmountOfSeedsLevel2,
            _ => _info.AmountOfSeedsLevel3,
        };

        float inicialAngle = _info.SeedInicialPosition;

        float angle = inicialAngle - (360f / amountOfSeeds) * _father.listOfSeeds.IndexOf(this);

        float currentRingRotation = _father.transform.eulerAngles.z;

        angle -= currentRingRotation;

        float angleInRadians = Mathf.Deg2Rad * angle;

        float x = radius * Mathf.Cos(angleInRadians);
        float y = radius * Mathf.Sin(angleInRadians);

        return new Vector3(x, y, 0);
    }

    IEnumerator Move(Vector3 finalPosition) {
        transform.SetParent(null);

        Vector3 direction = (finalPosition - transform.position).normalized;
        while (Vector3.Distance(transform.position, finalPosition) >= 0.2f) {
            transform.position += (_info.SeedSpeed * Time.deltaTime * direction);
            yield return null;
        }

        Explode();
    }

    private void Explode() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation);

        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 2);

        if (_level == 4) {
            Invoke(nameof(ExplodeAgain), _info.ExplosionsInterval);
        }

        ReturnObject();
    }

    private void ExplodeAgain() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation);

        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 2);
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        Explode();
    }
}
