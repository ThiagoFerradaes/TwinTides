using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUiManager : MonoBehaviour {
    [Header("Sprites")]
    [SerializeField] Sprite maevisSprite;
    [SerializeField] Sprite melSprite;
    [SerializeField] Sprite noSkillSprite;

    [Header("Images to receive Sprites")]
    [SerializeField] Image characterImage;
    [SerializeField] Image playerTwoCharacterImage;
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
    [SerializeField] TextMeshProUGUI playerTwoHealthText;
    [SerializeField] TextMeshProUGUI characterManaText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI npcSkillOneCooldownText;
    [SerializeField] TextMeshProUGUI npcSkillTwoCooldownText;
    [SerializeField] TextMeshProUGUI legendaryRelicSkillCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillOneCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillTwoCooldownText;
    [SerializeField] TextMeshProUGUI attackSkillCooldownText;

    [Header("Player info")]
    [SerializeField] GameObject maevisGameObject;
    [SerializeField] GameObject melGameObject;
    private GameObject _playerCharacter;
    private GameObject _playerTwoCharacter;

    readonly Dictionary<SkillType,TextMeshProUGUI> _listOfCooldowns = new();

    private void Start() {
        AddTextsToListOfCooldowns(); // Criar um dicionario com os textMeshPros
        SetCharacterSpriteInfo(); // Colocar a foto do personagem principal
        SetSkillsSpritesInfo(); // Colocar sprite nas skills
        SetInicialCooldowns(); // Zerar o texto de cooldowns
        SetGoldText(); // Mudar o texto do gold
        SetCharacterHealthManagerInfo();
        // Colocar o evento que chama o SetCooldown
    }
    void UpdatePlayerHealth((float maxHealth, float currentHealth) health) {
        characterHealthText.text = health.currentHealth.ToString("F0") + " / " + health.maxHealth.ToString("F0");
    }
    void UpdatePlayerTwoHealth((float maxHealth, float currentHealth) health) {
        playerTwoHealthText.text = health.currentHealth.ToString("F0") + " / " + health.maxHealth.ToString("F0");
    }

    private void AddTextsToListOfCooldowns() {
        _listOfCooldowns.Add(SkillType.NpcSkillOne, npcSkillOneCooldownText);
        _listOfCooldowns.Add(SkillType.NpcSkillTwo, npcSkillTwoCooldownText);
        _listOfCooldowns.Add(SkillType.LegendaryRelic, legendaryRelicSkillCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicOne, commonRelicSkillOneCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicTwo, commonRelicSkillTwoCooldownText);
        _listOfCooldowns.Add(SkillType.Attack, attackSkillCooldownText);
    }
    private void SetCharacterSpriteInfo() {
        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            if (melSprite != null && maevisSprite != null) {
                characterImage.sprite = maevisSprite;
                playerTwoCharacterImage.sprite = melSprite;
            }
        }
        else {
            if (melSprite != null && maevisSprite != null) {
                characterImage.sprite = melSprite;
                playerTwoCharacterImage.sprite = maevisSprite;
            }
        }
    }
    private void SetCharacterHealthManagerInfo() {
        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            _playerCharacter = maevisGameObject;
            _playerTwoCharacter = melGameObject;
            _playerCharacter.GetComponent<HealthManager>().UpdateHealth += UpdatePlayerHealth;
            _playerTwoCharacter.GetComponent<HealthManager>().UpdateHealth += UpdatePlayerTwoHealth;
        }
        else {
            _playerCharacter = melGameObject;
            _playerTwoCharacter = maevisGameObject;
            _playerCharacter.GetComponent<HealthManager>().UpdateHealth += UpdatePlayerHealth;
            _playerTwoCharacter.GetComponent<HealthManager>().UpdateHealth += UpdatePlayerTwoHealth;
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
    private void SetInicialCooldowns() {
        foreach (var item in _listOfCooldowns)
        {
            item.Value.text = "";
        }
    }
    private void SetGoldText() {
        goldText.text = LocalWhiteBoard.Instance.Gold.ToString("F0");
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
    private void OnDestroy() {
        _playerCharacter.GetComponent<HealthManager>().UpdateHealth -= UpdatePlayerHealth;
        _playerTwoCharacter.GetComponent<HealthManager>().UpdateHealth -= UpdatePlayerTwoHealth;
    }
}
