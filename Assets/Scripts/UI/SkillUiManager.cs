using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUiManager : MonoBehaviour {
    [Header("Sprites")]
    [SerializeField] Sprite maevisSprite;
    [SerializeField] Sprite molySprite;
    [SerializeField] Sprite noSkillSprite;

    [Header("Images to receive Sprites")]
    [SerializeField] Image characterImage;
    [SerializeField] Image characterHealthImage;
    [SerializeField] Image characterManaImage;
    [SerializeField] Image npcSkillOneImage;
    [SerializeField] Image npcSkillTwoImage;
    [SerializeField] Image legendaryRelicSkillImage;
    [SerializeField] Image commonRelicSkillOneImage;
    [SerializeField] Image commonRelicSkillTwoImage;
    [SerializeField] Image attackSkillImage;

    [Header("Images to show cooldown")]
    [SerializeField] Image npcSkillOneCooldownImage;
    [SerializeField] Image npcSkillTwoCooldownImage;
    [SerializeField] Image legendaryRelicSkillCooldownImage;
    [SerializeField] Image commonRelicSkillOneCooldownImage;
    [SerializeField] Image commonRelicSkillTwoCooldownImage;
    [SerializeField] Image attackSkillCooldownImage;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI characterHealthText;
    [SerializeField] TextMeshProUGUI characterManaText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI npcSkillOneCooldownText;
    [SerializeField] TextMeshProUGUI npcSkillTwoCooldownText;
    [SerializeField] TextMeshProUGUI legendaryRelicSkillCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillOneCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillTwoCooldownText;
    [SerializeField] TextMeshProUGUI attackSkillCooldownText;

    readonly Dictionary<SkillType,TextMeshProUGUI> _listOfCooldowns = new();

    private void OnEnable() {
        AddTextsToList();
        SetCharacterSpriteInfo();
        SetSkillsSpritesInfo();

        // Colocar o evento que chama o SetCooldown
    }
    private void AddTextsToList() {
        _listOfCooldowns.Add(SkillType.NpcSkillOne, npcSkillOneCooldownText);
        _listOfCooldowns.Add(SkillType.NpcSkillOne, npcSkillTwoCooldownText);
        _listOfCooldowns.Add(SkillType.LegendaryRelic, legendaryRelicSkillCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicOne, commonRelicSkillOneCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicTwo, commonRelicSkillTwoCooldownText);
        _listOfCooldowns.Add(SkillType.Attack, attackSkillCooldownText);
    }
    private void SetCharacterSpriteInfo() {
        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            characterImage.sprite = maevisSprite;
        }
        else {
            characterImage.sprite = molySprite;
        }
    }
    private void SetSkillsSpritesInfo() {
        if (LocalWhiteBoard.Instance.PlayerNpcSkillOne != null) {
            npcSkillOneImage.sprite = LocalWhiteBoard.Instance.PlayerNpcSkillOne.UiSprite;
        }
        else {
            npcSkillOneImage.sprite = noSkillSprite;
        }
        if (LocalWhiteBoard.Instance.PlayerNpcSkillTwo != null) {
            npcSkillTwoImage.sprite = LocalWhiteBoard.Instance.PlayerNpcSkillTwo.UiSprite;
        }
        else {
            npcSkillTwoImage.sprite = noSkillSprite;
        }

        if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null) {
            legendaryRelicSkillImage.sprite = LocalWhiteBoard.Instance.PlayerLegendarySkill.UiSprite;
        }
        else {
            legendaryRelicSkillImage.sprite = noSkillSprite;
        }
        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
            commonRelicSkillOneImage.sprite = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.UiSprite;
        }
        else {
            commonRelicSkillOneImage.sprite = noSkillSprite;
        }
        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null) {
            commonRelicSkillTwoImage.sprite = LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo.UiSprite;
        }
        else {
            commonRelicSkillTwoImage.sprite = noSkillSprite;
        }

        if (LocalWhiteBoard.Instance.PlayerAttackSkill != null) {
            attackSkillImage.sprite = LocalWhiteBoard.Instance.PlayerAttackSkill.UiSprite;
        }
        else {
            attackSkillImage.sprite = noSkillSprite;
        }
    }

    void SetCooldown(SkillType skillType, float cooldown) {
        if (_listOfCooldowns.ContainsKey(skillType)) {
            StartCoroutine(StartSkillCooldown(_listOfCooldowns[skillType], cooldown));
        }
    }

    IEnumerator StartSkillCooldown(TextMeshProUGUI text, float cooldown) {
        while (cooldown > 0) {
            cooldown -= Time.deltaTime;
            text.text = cooldown.ToString("F0");
            yield return null;
        }

        text.text = "";
    }
}
