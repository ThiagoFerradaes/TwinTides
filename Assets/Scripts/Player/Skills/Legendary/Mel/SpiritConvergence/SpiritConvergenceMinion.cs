using UnityEngine;

public class SpiritConvergenceMinion : SkillObjectPrefab
{
    SpiritConvergence _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    HealthManager _hManager;
    MovementManager _mManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpiritConvergence;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _mManager = GetComponent<MovementManager>();
            _hManager = GetComponent<HealthManager>();
        }

        Initiate();
    }

    void Initiate() {
        _hManager.RestoreAllHealthServerRpc();
    }


}
