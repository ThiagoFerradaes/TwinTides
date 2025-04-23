using System;
using System.Collections;
using UnityEngine;

public class SpectralSeedsObject : SkillObjectPrefab {

    SpectralSeeds _info;
    int _level;
    SkillContext _contex;
    SpectralSeedsRing _father;
    GameObject _mel;
    bool _active;

    public static event EventHandler OnSphereMoved;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {

        _info = info as SpectralSeeds;
        _level = skillLevel;
        _contex = context;

        if (_mel == null) _mel = PlayerSkillPooling.Instance.MelGameObject;
        if (_father == null) _father = GameObject.FindAnyObjectByType<SpectralSeedsRing>();

        TurnSeedOn();
    }

    void TurnSeedOn() {

        transform.localScale = _info.SeedSize;

        _father.listOfSeeds.Add(this);

        transform.SetParent(_father.transform);

        transform.SetLocalPositionAndRotation(GetPosition(), _contex.PlayerRotation);

        gameObject.SetActive(true);

        MelNormalAttackObject.OnNormalAttack += MelNormalAttackObject_OnNormalAttack;
        MelNormalAttackObject.OnNormalAttackHit += MelNormalAttackObject_OnNormalAttackHit;
    }

    private void MelNormalAttackObject_OnNormalAttackHit(object sender, EventArgs e) {
        if (!_active) return;
        StopAllCoroutines();
        StartCoroutine(ExplodeCoroutine());
    }

    private void MelNormalAttackObject_OnNormalAttack(object sender, MelNormalAttackObject.NormalAtackEventArgs e) {
        if (!gameObject.activeSelf) return;
        if (_father.listOfSeeds.IndexOf(this) == 0) {
            StartCoroutine(Move(e.FinalPosition));
            StartCoroutine(WaitForUpdateNextSpeed());
            _active = true;
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

        float speed = _mel.GetComponent<DamageManager>().ReturnMultipliedAttackSpeed(_info.SeedSpeed);

        while (Vector3.Distance(transform.position, finalPosition) >= 0.2f) {
            transform.position += (speed * Time.deltaTime * direction);
            yield return null;
        }

        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine() {
        Explode();

        if (_level == 4) {
            yield return new WaitForSeconds(_info.ExplosionsInterval);

            Explode();
        }

        End();
    }

    private void Explode() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _contex.SkillIdInUI);

        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 2);
    }

    public void End() {

        MelNormalAttackObject.OnNormalAttack -= MelNormalAttackObject_OnNormalAttack;

        MelNormalAttackObject.OnNormalAttackHit -= MelNormalAttackObject_OnNormalAttackHit;

        _active = false;

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
