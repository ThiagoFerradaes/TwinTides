using System;
using System.Collections;
using UnityEngine;

public class MelNormalAttackObject : SkillObjectPrefab {

    MelNormalAttack _info;
    SkillContext _context;

    public static event EventHandler<NormalAtackEventArgs> OnNormalAttack;
    public static event EventHandler OnNormalAttackHit;
    GameObject _mel;
    DamageManager _dManager;

    public class NormalAtackEventArgs : EventArgs {
        public Vector3 FinalPosition;

        public NormalAtackEventArgs(Vector3 finalPosition) {
            FinalPosition = finalPosition;
        }
    }

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MelNormalAttack;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        _dManager = _mel.GetComponent<DamageManager>();

        DefineSizeAndPosition();

        float cooldown = _dManager.ReturnDivisionAttackSpeed(_info.Cooldown);
        _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, cooldown);
    }

    private void DefineSizeAndPosition() {

        transform.localScale = _info.SphereSize;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (IsServer) StartCoroutine(Move());
    }

    IEnumerator Move() {

        transform.rotation = _mel.transform.rotation;

        float speed = _dManager.ReturnMultipliedAttackSpeed(_info.SphereSpeed);

        float duration = _info.SphereDistance / speed;

        Vector3 direction = transform.forward;
        Vector3 finalPosition = transform.position + duration * speed * direction;
        OnNormalAttack?.Invoke(this, new NormalAtackEventArgs(finalPosition));
        float startTime = Time.time;

        while(Time.time - startTime < duration) {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHelath)) return;

        enemyHelath.ApplyDamageOnServerRPC(_info.SphereDamage, true, true);

        OnNormalAttackHit?.Invoke(this, EventArgs.Empty);

        End();
    }

    void End() {
        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
