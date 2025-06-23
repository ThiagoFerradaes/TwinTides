using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;

public class SpectralSeedsRing : SkillObjectPrefab {
    SpectralSeeds _info;
    int _level;
    SkillContext _context;

    int _AmountOfSeeds;
    GameObject _mel;

    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;
    [HideInInspector] public List<SpectralSeedsObject> listOfSeeds = new();
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpectralSeeds;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            anim = _mel.GetComponentInChildren<Animator>();
            _pController = _mel.GetComponentInChildren<PlayerController>();
            _sManager = _mel.GetComponentInChildren<PlayerSkillManager>();
        }

        DefineSizeAndPosition();

        SpectralSeedsObject.OnSphereMoved += SpectralSeedsObject_OnSphereMoved;
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _info.RingSize;

        transform.SetParent(_mel.transform);

        transform.SetLocalPositionAndRotation(_info.RingPosition, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());

        StartCoroutine(Duration());
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("SpectralSeeds");

        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transição para animação nunca começou.");
                break;
            }
        }

        timer = 0f;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        InstantiateSeeds(); 

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
    }

    private void InstantiateSeeds() {
        _AmountOfSeeds = _level switch {
            1 => _info.AmountOfSeeds,
            2 => _info.AmountOfSeedsLevel2,
            _ => _info.AmountOfSeedsLevel3,
        };

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        for (int i = 0; i < _AmountOfSeeds; i++) {
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);
        }
    }
    void InstantiateOneSeed() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);
    }

    IEnumerator Duration() {
        float duration = _level < 4 ? _info.Duration : _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        foreach (var seed in listOfSeeds) {
            seed.transform.SetParent(null);
            seed.ReturnObject();
        }

        ReturnObject();
    }

    private void SpectralSeedsObject_OnSphereMoved(object sender, EventArgs e) {
        if (listOfSeeds.Count > 0) {
            StartCoroutine(UpdateRotation());
        }
        else {
            ReturnObject();
        }
    }

    IEnumerator UpdateRotation() {
        float anglePerSeed = 360f / _AmountOfSeeds;

        Quaternion inicialRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z + anglePerSeed);

        float elapsedTime = 0f;
        float duration = _mel.GetComponent<DamageManager>().ReturnDivisionAttackSpeed(_info.RingRotationDuration);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.localRotation = Quaternion.Lerp(inicialRotation, targetRotation, t);
            yield return null;
        }

        transform.localRotation = targetRotation;

        yield return null;

        if (_level == 4 && listOfSeeds.Count < 8) {
            InstantiateOneSeed();
        }
    }
    public override void ReturnObject() {
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        SpectralSeedsObject.OnSphereMoved -= SpectralSeedsObject_OnSphereMoved;

        listOfSeeds.Clear();

        Cooldown();

        base.ReturnObject();
    }

    void Cooldown() {
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter)
            _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
