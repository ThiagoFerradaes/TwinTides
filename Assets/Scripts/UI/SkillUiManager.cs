using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillUiManager : MonoBehaviour {


    #region Variables

    [Header("Sprites")]
    [SerializeField] Sprite maevisSprite;
    [SerializeField] Sprite melSprite;
    [SerializeField] Sprite noSkillSprite;

    [Header("Images to receive Sprites")]
    [SerializeField] Image characterImage;
    [SerializeField] Image playerTwoCharacterImage;
    [SerializeField] Image characterHealthImage;
    [SerializeField] Image characterManaImage;
    [SerializeField] Image legendaryRelicSkillImage;
    [SerializeField] Image commonRelicSkillOneImage;
    [SerializeField] Image commonRelicSkillTwoImage;
    [SerializeField] Image attackSkillImage;
    [SerializeField] Image dashImage;

    [Header("Images to show cooldown")]
    [SerializeField] Image legendaryRelicSkillCooldownImage;
    [SerializeField] Image commonRelicSkillOneCooldownImage;
    [SerializeField] Image commonRelicSkillTwoCooldownImage;
    [SerializeField] Image attackSkillCooldownImage;
    [SerializeField] Image dashCooldownImage;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI characterHealthText;
    [SerializeField] TextMeshProUGUI playerTwoHealthText;
    [SerializeField] TextMeshProUGUI playerShieldText;
    [SerializeField] TextMeshProUGUI playerTwoShieldText;
    [SerializeField] TextMeshProUGUI characterManaText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI legendaryRelicSkillCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillOneCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillTwoCooldownText;
    [SerializeField] TextMeshProUGUI attackSkillCooldownText;
    [SerializeField] TextMeshProUGUI dashCooldownText;

    [Header("Player two Info")]
    [SerializeField] GameObject playerTwoInfo;

    private GameObject _playerCharacter;
    private GameObject _playerTwoCharacter;

    readonly Dictionary<SkillType, TextMeshProUGUI> _listOfCooldowns = new();
    #endregion

    #region Methods
    private void Start() {
        AddTextsToListOfCooldowns(); // Criar um dicionario com os textMeshPros
        SetCharacterSpriteInfo(); // Colocar a foto do personagem principal
        SetSkillsSpritesInfo(); // Colocar sprite nas skills
        SetInicialCooldowns(); // Zerar o texto de cooldowns
        SetGoldText(); // Mudar o texto do gold
    }
    private void OnEnable() {
        PlayerSetUp.OnPlayerSpawned += SetPlayerOne;
        PlayerSetUp.OnPlayerTwoSpawned += SetPlayerTwo;
    }
    void SetPlayerOne(GameObject player) {
        _playerCharacter = player;

        SetCharacterHealthManagerInfo();
        _playerCharacter.GetComponent<PlayerController>().OnDashCooldown += SetCooldown;
        PlayerSkillManager skillManager = _playerCharacter.GetComponent<PlayerSkillManager>();
        skillManager.OnBaseAttack += SkillManager_SkillUSed;
        skillManager.OnCommonSkillOne += SkillManager_SkillUSed;
        skillManager.OnCommonSkillTwo += SkillManager_SkillUSed;
        skillManager.OnLegendary += SkillManager_SkillUSed;
    }

    private void SkillManager_SkillUSed(object sender, PlayerSkillManager.SkillEventHandler e) {
        SetCooldown(e.Type, e.SkillCooldown);
    }

    void SetPlayerTwo(GameObject player) {
        _playerTwoCharacter = player;

        SetSecondCharacterHealthManagerInfo();
    }
    void UpdatePlayerHealth((float maxHealth, float currentHealth, float currentShield) health) {
        characterHealthText.text = health.currentHealth.ToString("F0") + " / " + health.maxHealth.ToString("F0");
        if (health.currentShield != 0) playerShieldText.text = health.currentShield.ToString("F0");
        else playerShieldText.text = "";
    }
    void UpdatePlayerTwoHealth((float maxHealth, float currentHealth, float currentShield) health) {
        playerTwoHealthText.text = health.currentHealth.ToString("F0") + " / " + health.maxHealth.ToString("F0");
        if (health.currentShield != 0) playerTwoShieldText.text = health.currentShield.ToString("F0");
        else playerTwoShieldText.text = "";
    }

    private void AddTextsToListOfCooldowns() {
        _listOfCooldowns.Add(SkillType.LegendaryRelic, legendaryRelicSkillCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicOne, commonRelicSkillOneCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicTwo, commonRelicSkillTwoCooldownText);
        _listOfCooldowns.Add(SkillType.Attack, attackSkillCooldownText);
        _listOfCooldowns.Add(SkillType.Dash, dashCooldownText);
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
        _playerCharacter.GetComponent<HealthManager>().UpdateHealth += UpdatePlayerHealth;
    }
    private void SetSecondCharacterHealthManagerInfo() {
        playerTwoInfo.SetActive(true);
        _playerTwoCharacter.GetComponent<HealthManager>().UpdateHealth += UpdatePlayerTwoHealth;
    }
    private void SetSkillsSpritesInfo() {

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
        foreach (var item in _listOfCooldowns) {
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
    private void OnDisable() {
        if (_playerCharacter == null || _playerTwoCharacter == null) return;
        _playerCharacter.GetComponent<HealthManager>().UpdateHealth -= UpdatePlayerHealth;
        _playerTwoCharacter.GetComponent<HealthManager>().UpdateHealth -= UpdatePlayerTwoHealth;

        PlayerSetUp.OnPlayerSpawned -= SetPlayerOne;
        PlayerSetUp.OnPlayerTwoSpawned -= SetPlayerTwo;
        _playerCharacter.GetComponent<PlayerController>().OnDashCooldown -= SetCooldown;
    }
    #endregion
}
