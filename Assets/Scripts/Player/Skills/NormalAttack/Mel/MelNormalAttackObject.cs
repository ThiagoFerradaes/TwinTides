using System;
using System.Collections;
using UnityEngine;

public class MelNormalAttackObject : SkillObjectPrefab {

    MelNormalAttack _info;
    SkillContext _context;

    public static event EventHandler<NormalAtackEventArgs> OnNormalAttack;
    GameObject _mel;

    public class NormalAtackEventArgs : EventArgs {
        public Vector3 FinalPosition;

        public NormalAtackEventArgs(Vector3 finalPosition) {
            FinalPosition = finalPosition;
        }
    }

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MelNormalAttack;
        _context = context;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        if (_mel == null) {
            _mel = GameObject.FindGameObjectWithTag("Mel");
        }

        _mel.GetComponent<PlayerController>().StartAimMode();

        transform.localScale = _info.SphereSize;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Move());
    }

    IEnumerator Move() {

        transform.rotation = _mel.transform.rotation;

        Vector3 direction = transform.forward;
        Vector3 finalPosition = transform.position + _info.SphereDuration * _info.SphereSpeed * direction;
        OnNormalAttack?.Invoke(this, new NormalAtackEventArgs(finalPosition));
        float startTime = Time.time;

        while(Time.time - startTime < _info.SphereDuration) {
            transform.Translate(_info.SphereSpeed * Time.deltaTime * Vector3.forward);
            yield return null;
        }

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHelath)) return;

        enemyHelath.ApplyDamageOnServerRPC(_info.SphereDamage, true, true);
    }
}
