using FMODUnity;
using System.Collections;
using UnityEngine;

public class WarCryExplosion : SkillObjectPrefab {
    Warcry _info;
    int _level;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Warcry;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _level < 3 ? Vector3.one * _info.ExplosionRadius : Vector3.one * _info.ExplosionRadiusLevel3;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.CrySound.IsNull) RuntimeManager.PlayOneShot(_info.CrySound);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Mel") && _level > 1) {

            if (_level > 3) {
                other.GetComponent<DamageManager>().IncreaseAttackSpeedWithTime(_info.PercentAttackSpeedLevel4, _info.DurationLevel4);
            }
            else {
                other.GetComponent<DamageManager>().IncreaseAttackSpeedWithTime(_info.PercentAttackSpeedLevel2, _info.Duration);
            }

            if (_level > 2) {
                other.GetComponent<MovementManager>().IncreaseMoveSpeedWithTime(_info.PercentMoveSpeedGain, _info.Duration);
            }
        }

        if (other.CompareTag("Enemy") && _level > 2) {
            other.GetComponent<MovementManager>().StunWithTime(_info.StunDuration);
        }
    }
}
