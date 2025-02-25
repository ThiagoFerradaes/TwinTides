using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpectralSeedsRing : SkillObjectPrefab {
    SpectralSeeds _info;
    int _level;
    SkillContext _context;

    int _AmountOfSeeds;
    GameObject _mel;

    [HideInInspector] public List<SpectralSeedsObject> listOfSeeds = new();
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpectralSeeds;
        _level = skillLevel;
        _context = context;

        if (_mel == null) _mel = PlayerSkillPooling.Instance.MelGameObject;

        DefineSizeAndPosition();

        SpectralSeedsObject.OnSphereMoved += SpectralSeedsObject_OnSphereMoved;
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _info.RingSize;

        if (IsServer) {
            transform.SetParent(_mel.transform);

            transform.SetLocalPositionAndRotation(_info.RingPosition, Quaternion.Euler(0, 0, 0));
        }

        gameObject.SetActive(true);

        InstantiateSeeds();

        StartCoroutine(Duration());
    }

    private void InstantiateSeeds() {
        _AmountOfSeeds = _level switch {
            1 => _info.AmountOfSeeds,
            2 => _info.AmountOfSeedsLevel2,
            _ => _info.AmountOfSeedsLevel3,
        };
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        for (int i = 0; i < _AmountOfSeeds; i++) {
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
        }
    }
    void InstantiateOneSeed() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
    }

    IEnumerator Duration() {
        float duration = _level < 4 ? _info.Duration : _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        foreach (var seed in listOfSeeds) {
            seed.transform.SetParent(null);
            seed.End();
        }

        End();
    }

    private void SpectralSeedsObject_OnSphereMoved(object sender, EventArgs e) {
        if (listOfSeeds.Count > 0) {
            StartCoroutine(UpdateRotation());
        }
        else {
            End();
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
    void End() {
        SpectralSeedsObject.OnSphereMoved -= SpectralSeedsObject_OnSphereMoved;

        listOfSeeds.Clear();

        Cooldown();

        ReturnObject();
    }

    void Cooldown() {
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter)
            _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
