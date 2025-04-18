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
    [SerializeField] Image characterHealthImage;
    [SerializeField] Image legendaryRelicSkillImage;
    [SerializeField] Image commonRelicSkillOneImage;
    [SerializeField] Image commonRelicSkillTwoImage;

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
    Coroutine baseAttackCoroutine, commonRelicOneCoroutine, commonRelicTwoCoroutine, legendaryRelicCoroutine;
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
        LocalWhiteBoard.OnRelicEquiped += LocalWhiteBoard_OnRelicEquiped;
        LocalWhiteBoard.OnGoldChanged += LocalWhiteBoard_OnGoldChanged;
    }

    private void LocalWhiteBoard_OnGoldChanged(object sender, EventArgs e) {
        SetGoldText();
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
        Debug.Log("Event invoked");
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

    }
    private void SetCharacterSpriteInfo() {
        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            if (melSprite != null && maevisSprite != null) {
                characterImage.sprite = maevisSprite;
                characterTag.sprite = maevisTag;

                playerTwoCharacterImage.sprite = melSprite;
                playerTwoCharacterTag.sprite = melTag;
            }
        }
        else {
            if (melSprite != null && maevisSprite != null) {
                characterImage.sprite = melSprite;
                characterTag.sprite = melTag;

                playerTwoCharacterImage.sprite = maevisSprite;
                playerTwoCharacterTag.sprite = maevisTag;
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
    private void LocalWhiteBoard_OnRelicEquiped(object sender, EventArgs e) {
        SetSkillsSpritesInfo();
    }
    private void SetSkillsSpritesInfo() {

        if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null) {
            if (!legendaryRelicSkillImage.gameObject.activeSelf) legendaryRelicSkillImage.gameObject.SetActive(true);
            legendaryRelicSkillImage.sprite = LocalWhiteBoard.Instance.PlayerLegendarySkill.UiSprite;
        }
        else {
            legendaryRelicSkillImage.gameObject.SetActive(false);
            legendaryRelicSkillImage.sprite = null;
        }
        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
            commonRelicSkillOneImage.sprite = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne.UiSprite;
            if (!commonRelicSkillOneImage.gameObject.activeSelf) commonRelicSkillOneImage.gameObject.SetActive(true);
        }
        else {
            commonRelicSkillOneImage.gameObject.SetActive(false);
            commonRelicSkillOneImage.sprite = null;
        }
        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null) {
            commonRelicSkillTwoImage.sprite = LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo.UiSprite;
            if (!commonRelicSkillTwoImage.gameObject.activeSelf) commonRelicSkillTwoImage.gameObject.SetActive(true);
        }
        else {
            commonRelicSkillTwoImage.gameObject.SetActive(false);
            commonRelicSkillTwoImage.sprite = null;
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
            switch (skillType) {
                //case SkillType.Attack:
                //    if (baseAttackCoroutine != null) {
                //        StopCoroutine(baseAttackCoroutine);
                //        baseAttackCoroutine = null;
                //    }
                //    baseAttackCoroutine = StartCoroutine(StartSkillCooldown(_listOfCooldowns[skillType], cooldown));
                //    break;
                case SkillType.CommonRelicOne:
                    if (commonRelicOneCoroutine != null) {
                        StopCoroutine(commonRelicOneCoroutine);
                        commonRelicOneCoroutine = null;
                    }
                    commonRelicOneCoroutine = StartCoroutine(StartSkillCooldown(_listOfCooldowns[skillType], cooldown, commonRelicSkillOneCooldownImage));
                    break;
                case SkillType.CommonRelicTwo:
                    if (commonRelicTwoCoroutine != null) {
                        StopCoroutine(commonRelicTwoCoroutine);
                        commonRelicTwoCoroutine = null;
                    }
                    commonRelicTwoCoroutine = StartCoroutine(StartSkillCooldown(_listOfCooldowns[skillType], cooldown, commonRelicSkillTwoCooldownImage));
                    break;
                case SkillType.LegendaryRelic:
                    if (legendaryRelicCoroutine != null) {
                        StopCoroutine(legendaryRelicCoroutine);
                        legendaryRelicCoroutine = null;
                    }
                    legendaryRelicCoroutine = StartCoroutine(StartSkillCooldown(_listOfCooldowns[skillType], cooldown, legendaryRelicSkillCooldownImage));
                    break;
            }
        }
    }

    IEnumerator StartSkillCooldown(TextMeshProUGUI text, float cooldown, Image cooldownImage) {
        float skillCooldown = cooldown;
        if (!cooldownImage.gameObject.activeSelf) cooldownImage.gameObject.SetActive(true);
        while (cooldown > 0) {
            cooldown -= Time.deltaTime;
            text.text = cooldown.ToString("F0");
            if (cooldownImage != null) cooldownImage.fillAmount = cooldown / skillCooldown;
            yield return null;
        }

        cooldownImage.gameObject.SetActive(false);
        text.text = "";
    }
    private void OnDisable() {
        if (_playerCharacter == null || _playerTwoCharacter == null) return;
        _playerCharacter.GetComponent<HealthManager>().UpdateHealth -= UpdatePlayerHealth;
        _playerTwoCharacter.GetComponent<HealthManager>().UpdateHealth -= UpdatePlayerTwoHealth;

        PlayerSetUp.OnPlayerSpawned -= SetPlayerOne;
        PlayerSetUp.OnPlayerTwoSpawned -= SetPlayerTwo;
        LocalWhiteBoard.OnRelicEquiped -= LocalWhiteBoard_OnRelicEquiped;
        _playerCharacter.GetComponent<PlayerController>().OnDashCooldown -= SetCooldown;
    }
    #endregion
}
