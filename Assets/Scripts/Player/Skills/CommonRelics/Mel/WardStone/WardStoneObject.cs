using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class WardStoneObject : SkillObjectPrefab {
    WardStone _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as WardStone;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefineSizeAndPosition();

    }

    private void DefineSizeAndPosition() {
        transform.localScale = _level < 3 ? _info.ExplosionRadius : _info.ExplosionRadiusLevel3;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.ExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.ExplosionSound, transform.position);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        CreateArea();

        ReturnObject();
    }

    private void CreateArea() {
        if (_level >= 3 && LocalWhiteBoard.Instance.PlayerCharacter == Characters.Mel) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Mel") || other.CompareTag("Maevis")) {

            if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

            if (_level == 4) {
                health.AddBuffToList(_info.BuffblockerLevel4);
                health.AddBuffToList(_info.HealingIncreaseBuffLevel4);
                health.AddBuffToList(_info.ShieldIncreaseBuffLevel4);
            }
            else if (_level >= 2) {
                health.AddBuffToList(_info.Debuffblocker);
                health.AddBuffToList(_info.HealingIncreaseBuff);
                health.AddBuffToList(_info.ShieldIncreaseBuff);

                health.ApplyShield(_info.AmountOfShield, _info.ShieldDuration, true);
            }
            else {
                health.AddBuffToList(_info.Debuffblocker);
            }

            health.Heal(_info.BaseHealing, true);
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
