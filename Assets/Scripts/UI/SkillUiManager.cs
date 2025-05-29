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
    [SerializeField] Sprite maevisTag;
    [SerializeField] Sprite melTag;

    [Header("Images to receive Sprites")]
    [SerializeField] Image characterImage;
    [SerializeField] Image characterTag;
    [SerializeField] Image playerTwoCharacterImage;
    [SerializeField] Image playerTwoCharacterTag;
    [SerializeField] Image legendaryRelicSkillImage;
    [SerializeField] Image commonRelicSkillOneImage;
    [SerializeField] Image commonRelicSkillTwoImage;

    [Header("Health And Shield Images")]
    [SerializeField] Image characterHealthImage;
    [SerializeField] Image characterShieldImage;
    [SerializeField] Image characterTwoHealthImage;
    [SerializeField] Image characterTwoShieldImage;

    [Header("Images to show cooldown")]
    [SerializeField] Image legendaryRelicSkillCooldownImage;
    [SerializeField] Image commonRelicSkillOneCooldownImage;
    [SerializeField] Image commonRelicSkillTwoCooldownImage;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI characterHealthText;
    [SerializeField] TextMeshProUGUI playerTwoHealthText;
    [SerializeField] TextMeshProUGUI playerShieldText;
    [SerializeField] TextMeshProUGUI playerTwoShieldText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI legendaryRelicSkillCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillOneCooldownText;
    [SerializeField] TextMeshProUGUI commonRelicSkillTwoCooldownText;

    [Header("Player two Info")]
    [SerializeField] GameObject playerTwoInfo;

    private GameObject _playerCharacter;
    private GameObject _playerTwoCharacter;

    readonly Dictionary<SkillType, TextMeshProUGUI> _listOfCooldowns = new();

    // Corrotinas
    Coroutine commonRelicOneCoroutine, commonRelicTwoCoroutine, legendaryRelicCoroutine;
    #endregion

    #region Methods

    #region Initialize
    private void Start() {
        AddTextsToListOfCooldowns(); // Criar um dicionario com os textMeshPros
        SetCharacterSpriteInfo(); // Colocar a foto do personagem principal
        SetSkillsSpritesInfo(); // Colocar sprite nas skills
        SetInicialCooldowns(); // Zerar o texto de cooldowns
        UpdateGoldText(); // Mudar o texto do gold
    }
    private void OnEnable() {
        PlayerSetUp.OnPlayerSpawned += SetPlayerOne;
        PlayerSetUp.OnPlayerTwoSpawned += SetPlayerTwo;
        LocalWhiteBoard.OnRelicEquiped += OnRelicEquipped;
        LocalWhiteBoard.OnGoldChanged += OnGoldChanged;
    }
    private void OnDisable() {
        if (_playerCharacter != null) {
            _playerCharacter.GetComponent<HealthManager>().OnHealthUpdate -= UpdatePlayerHealth;
        }

        if (_playerTwoCharacter != null) {
            _playerTwoCharacter.GetComponent<HealthManager>().OnHealthUpdate -= UpdatePlayerTwoHealth;
        }

        PlayerSetUp.OnPlayerSpawned -= SetPlayerOne;
        PlayerSetUp.OnPlayerTwoSpawned -= SetPlayerTwo;
        LocalWhiteBoard.OnRelicEquiped -= OnRelicEquipped;
        LocalWhiteBoard.OnGoldChanged -= OnGoldChanged;
    }

    #endregion

    #region Set Players
    void SetPlayerOne(GameObject player) {
        _playerCharacter = player;

        SetCharacterHealthManagerInfo();

        PlayerSkillManager skillManager = _playerCharacter.GetComponent<PlayerSkillManager>();
        skillManager.OnCommonSkillOne += SkillManager_SkillUSed;
        skillManager.OnCommonSkillTwo += SkillManager_SkillUSed;
        skillManager.OnLegendary += SkillManager_SkillUSed;
    }

    void SetPlayerTwo(GameObject player) {
        _playerTwoCharacter = player;

        SetSecondCharacterHealthManagerInfo();
    }
    #endregion

    #region Player Health
    private void UpdatePlayerHealth((float maxHealth, float currentHealth, float currentShield, float maxShield) health) {

        // Textos
        characterHealthText.text = $"{health.currentHealth:F0} / {health.maxHealth:F0}";
        playerShieldText.text = health.currentShield > 0 ? health.currentShield.ToString("F0") : "";

        // Imagens
        characterHealthImage.fillAmount = health.currentHealth / health.maxHealth;
        characterShieldImage.fillAmount = health.currentShield / health.maxShield;
    }

    private void UpdatePlayerTwoHealth((float maxHealth, float currentHealth, float currentShield, float maxShield) health) {
        // Textos
        playerTwoHealthText.text = $"{health.currentHealth:F0} / {health.maxHealth:F0}";
        playerTwoShieldText.text = health.currentShield > 0 ? health.currentShield.ToString("F0") : "";

        // Imagens
        characterTwoHealthImage.fillAmount = health.currentHealth / health.maxHealth;
        characterTwoShieldImage.fillAmount = health.currentShield / health.maxShield;
    }

    private void SetCharacterHealthManagerInfo() {
        HealthManager health = _playerCharacter.GetComponent<HealthManager>();
        health.OnHealthUpdate += UpdatePlayerHealth;

        UpdatePlayerHealth((health.ReturnMaxHealth(), health.ReturnCurrentHealth(), health.ReturnShieldAmount(), health.ReturnMaxShieldAmount()));
    }
    private void SetSecondCharacterHealthManagerInfo() {
        playerTwoInfo.SetActive(true);

        HealthManager health = _playerTwoCharacter.GetComponent<HealthManager>();
        health.OnHealthUpdate += UpdatePlayerTwoHealth;

        UpdatePlayerTwoHealth((health.ReturnMaxHealth(), health.ReturnCurrentHealth(), health.ReturnShieldAmount(), health.ReturnMaxShieldAmount()));
    }
    #endregion

    #region Sprite
    private void OnRelicEquipped(object sender, EventArgs e) {
        SetSkillsSpritesInfo();
    }
    private void SetCharacterSpriteInfo() {
        bool isMaevis = LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis;

        characterImage.sprite = isMaevis ? maevisSprite : melSprite;
        characterTag.sprite = isMaevis ? maevisTag : melTag;
        playerTwoCharacterImage.sprite = isMaevis ? melSprite : maevisSprite;
        playerTwoCharacterTag.sprite = isMaevis ? melTag : maevisTag;
    }

    private void SetSkillsSpritesInfo() {
        SetSkillSprite(legendaryRelicSkillImage, LocalWhiteBoard.Instance.PlayerLegendarySkill);
        SetSkillSprite(commonRelicSkillOneImage, LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne);
        SetSkillSprite(commonRelicSkillTwoImage, LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo);
    }

    private void SetSkillSprite(Image image, Skill skill) {
        if (skill != null) {
            image.sprite = skill.UiSprite;
            if (!image.gameObject.activeSelf) image.gameObject.SetActive(true);
        }
        else {
            image.sprite = null;
            image.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Gold
    private void OnGoldChanged(object sender, EventArgs e) {
        UpdateGoldText();
    }

    private void UpdateGoldText() {
        goldText.text = LocalWhiteBoard.Instance.Gold.ToString("F0");
    }
    #endregion

    #region Players Skills
    private void AddTextsToListOfCooldowns() {
        _listOfCooldowns.Add(SkillType.LegendaryRelic, legendaryRelicSkillCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicOne, commonRelicSkillOneCooldownText);
        _listOfCooldowns.Add(SkillType.CommonRelicTwo, commonRelicSkillTwoCooldownText);

    }
    private void SetInicialCooldowns() {
        foreach (var cooldown in _listOfCooldowns) {
            cooldown.Value.text = "";
        }
    }
    private void SkillManager_SkillUSed(object sender, PlayerSkillManager.SkillEventHandler e) {
        SetCooldown(e.Type, e.SkillCooldown);
    }

    private void SetCooldown(SkillType type, float cooldown) {
        if (!_listOfCooldowns.ContainsKey(type)) return;

        Coroutine coroutineToStop = null;
        Image cooldownImage = null;

        switch (type) {
            case SkillType.CommonRelicOne:
                coroutineToStop = commonRelicOneCoroutine;
                cooldownImage = commonRelicSkillOneCooldownImage;
                break;
            case SkillType.CommonRelicTwo:
                coroutineToStop = commonRelicTwoCoroutine;
                cooldownImage = commonRelicSkillTwoCooldownImage;
                break;
            case SkillType.LegendaryRelic:
                coroutineToStop = legendaryRelicCoroutine;
                cooldownImage = legendaryRelicSkillCooldownImage;
                break;
        }

        if (coroutineToStop != null) StopCoroutine(coroutineToStop);

        Coroutine newCoroutine = StartCoroutine(StartSkillCooldown(_listOfCooldowns[type], cooldown, cooldownImage));
        switch (type) {
            case SkillType.CommonRelicOne: commonRelicOneCoroutine = newCoroutine; break;
            case SkillType.CommonRelicTwo: commonRelicTwoCoroutine = newCoroutine; break;
            case SkillType.LegendaryRelic: legendaryRelicCoroutine = newCoroutine; break;
        }
    }
    private IEnumerator StartSkillCooldown(TextMeshProUGUI text, float duration, Image cooldownImage) {
        if (!cooldownImage.gameObject.activeSelf) cooldownImage.gameObject.SetActive(true);

        float timer = duration;
        while (timer > 0) {
            timer -= Time.deltaTime;
            text.text = timer.ToString("F0");
            if (cooldownImage != null) cooldownImage.fillAmount = timer / duration;
            yield return null;
        }

        cooldownImage.gameObject.SetActive(false);
        text.text = "";
    }
    #endregion

    #endregion
}
