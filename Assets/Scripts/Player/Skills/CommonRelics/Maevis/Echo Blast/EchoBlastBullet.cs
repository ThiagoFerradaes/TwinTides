using System;
using System.Collections;
using UnityEngine;

public class EchoBlastBullet : SkillObjectPrefab
{
    EchoBlast _info;
    int _level;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Move());
    }

    IEnumerator Move() {
        float elapsedTime = 0f;

        while (elapsedTime < _info.BulletDuration) {
            elapsedTime += Time.deltaTime;
            transform.position += _info.BulletSpeed * Time.deltaTime * transform.forward;
            yield return null;
        }

        ReturnObject();
    }
    

    private void OnTriggerEnter(Collider other) {
        if(!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        Explode();
    }
    void Explode() {
        if (IsServer) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);

            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 2);
        }

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

}
