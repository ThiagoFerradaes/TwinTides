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

        DefineSizeAndPosition();
        SpectralSeedsObject.OnSphereMoved += SpectralSeedsObject_OnSphereMoved;
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
        float targetAngle = transform.eulerAngles.z + anglePerSeed;

        Quaternion startRotation = transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(0, 0, targetAngle);

        float elapsedTime = 0f;

        while (elapsedTime < _info.RingRotationDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _info.RingRotationDuration;
            transform.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
            yield return null;
        }

        transform.rotation = finalRotation;

        yield return null;

        if (_level == 4 && listOfSeeds.Count < 8) {
            InstantiateOneSeed();
        }
    }

    private void DefineSizeAndPosition() {
        if (_mel == null) {
            _mel = GameObject.FindGameObjectWithTag("Mel");
            //_mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        transform.localScale = _info.RingSize;

        transform.SetParent(_mel.transform);

        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        transform.SetLocalPositionAndRotation(_info.RingPosition, rotation);

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
        if (_level < 4) {
            yield return new WaitForSeconds(_info.Duration);
        }
        else {
            yield return new WaitForSeconds(_info.DurationLevel4);
        }

        foreach (var seed in listOfSeeds) {
            seed.transform.SetParent(null);
            seed.ReturnObject();
        }
        End();
    }
    void End() {
        listOfSeeds.Clear();
        Cooldown();
        ReturnObject();
    }

    void Cooldown() {
        _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
