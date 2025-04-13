using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TotemManager : MonoBehaviour {
    public static TotemManager Instance;

    [SerializeField] GameObject totemScreen;
    [SerializeField] Button closeScreenButton;
    [SerializeField] Button equipTabButton, upgradeTabButton;
    [SerializeField] List<float> prices = new();

    [Header("Equip Tab")]
    [SerializeField] GameObject equipScreen;
    [SerializeField] Button firstRelicRightArrow, firstRelicLeftArrow, secondRelicRightArrow, secondRelicLeftArrow, legendaryRelicRightArrow, legendaryRelicLeftArrow;
    [SerializeField] TextMeshProUGUI firstRelicDescription, secondRelicDescription, legendaryRelicDescription;
    [SerializeField] Image firstRelicImage, secondRelicImage, legendaryRelicImage;

    [Header("Upgrade Tab")]
    [SerializeField] GameObject upgradeScreen;
    [SerializeField] Button upArrow, downArrow, buyButton;
    [SerializeField] TextMeshProUGUI upgradeDescription, buyPrice;
    [SerializeField] Image relicImage, firstUpgradeImage, secondUpgradeImage, thirdUpgradeImage, fourthUpgradeImage, buyButtonBackGroundImage;

    public List<CommonRelic> listOfCommonRelics = new();
    List<LegendaryRelic> listOfLegendaryRelics = new();
    int firstCommonIndex, secondCommonIndex, legendaryIndex, updateIndex;

    #region Initialize
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        SetButtons();
    }

    private void SetButtons() {
        // Screen

        closeScreenButton.onClick.AddListener(TurnTotemScreenOff);

        // Equip Tab

        equipTabButton.onClick.AddListener(TurnEquipTabOn);

        // Upgrade Tab

        upgradeTabButton.onClick.AddListener(TurnUpgradeTabOn);

        // Arrows
        firstRelicRightArrow.onClick.AddListener(() => EquipArrowFunction(SkillIndex.firstCommon, ArrowDirection.right));
        firstRelicLeftArrow.onClick.AddListener(() => EquipArrowFunction(SkillIndex.firstCommon, ArrowDirection.left));

        secondRelicRightArrow.onClick.AddListener(() => EquipArrowFunction(SkillIndex.secondCommon, ArrowDirection.right));
        secondRelicLeftArrow.onClick.AddListener(() => EquipArrowFunction(SkillIndex.secondCommon, ArrowDirection.left));

        legendaryRelicRightArrow.onClick.AddListener(() => EquipArrowFunction(SkillIndex.legendary, ArrowDirection.right));
        legendaryRelicLeftArrow.onClick.AddListener(() => EquipArrowFunction(SkillIndex.legendary, ArrowDirection.left));

        upArrow.onClick.AddListener(() => UpdateArrowFunction(ArrowDirection.up));
        downArrow.onClick.AddListener(() => UpdateArrowFunction(ArrowDirection.down));

        // Buy
        buyButton.onClick.AddListener(BuyButton);
    }

    public void TurnTotemScreenOn() {

        CreateAndVerifyList();

        DefineRelics();

        upgradeScreen.SetActive(false);
        equipScreen.SetActive(true);
        totemScreen.SetActive(true);

        LocalWhiteBoard.Instance.AnimationOn = true;
    }

    void CreateAndVerifyList() {
        listOfCommonRelics = LocalWhiteBoard.Instance.CommonRelicInventory.Keys.ToList<CommonRelic>();
        listOfLegendaryRelics = LocalWhiteBoard.Instance.LegendaryRelicInventory.Keys.ToList<LegendaryRelic>();

        if (listOfCommonRelics.Count > 1) {
            firstRelicLeftArrow.gameObject.SetActive(true);
            firstRelicRightArrow.gameObject.SetActive(true);

            secondRelicLeftArrow.gameObject.SetActive(true);
            secondRelicRightArrow.gameObject.SetActive(true);

            upArrow.gameObject.SetActive(true);
            downArrow.gameObject.SetActive(true);
        }
        else {
            firstRelicLeftArrow.gameObject.SetActive(false);
            firstRelicRightArrow.gameObject.SetActive(false);

            secondRelicLeftArrow.gameObject.SetActive(false);
            secondRelicRightArrow.gameObject.SetActive(false);

            upArrow.gameObject.SetActive(false);
            downArrow.gameObject.SetActive(false);
        }

        if (listOfLegendaryRelics.Count > 1) {
            legendaryRelicLeftArrow.gameObject.SetActive(true);
            legendaryRelicRightArrow.gameObject.SetActive(true);
        }
        else {
            legendaryRelicLeftArrow.gameObject.SetActive(false);
            legendaryRelicRightArrow.gameObject.SetActive(false);
        }
    }

    private void DefineRelics() {

        // First Relic

        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
            CommonRelic firstRelic = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne;
            firstCommonIndex = listOfCommonRelics.IndexOf(firstRelic);
            DefineImagenAndDescription(firstRelic, SkillIndex.firstCommon);
        }


        // Second Relic

        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null) {
            CommonRelic secondRelic = LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo;
            secondCommonIndex = listOfCommonRelics.IndexOf(secondRelic);
            DefineImagenAndDescription(secondRelic, SkillIndex.secondCommon);
        }

        // Legendary Relic

        if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null) {
            LegendaryRelic legendaryRelic = LocalWhiteBoard.Instance.PlayerLegendarySkill;
            legendaryIndex = listOfLegendaryRelics.IndexOf(legendaryRelic);
            DefineImagenAndDescription(legendaryRelic, SkillIndex.legendary);
        }

        // Upgrade
        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
            CommonRelic firstRelic = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne;
            updateIndex = listOfCommonRelics.IndexOf(firstRelic);
            relicImage.sprite = listOfCommonRelics[updateIndex].UiSprite;
            ChangeDescriptionPrinceAndImages();
        }
    }

    #endregion

    #region Equip Tab
    enum SkillIndex { firstCommon, secondCommon, legendary }
    private void DefineImagenAndDescription(Skill skill, SkillIndex indexOfSkill) {
        switch (indexOfSkill) {
            case SkillIndex.firstCommon:
                firstRelicImage.sprite = skill.UiSprite;
                firstRelicDescription.text = skill.SkillsDescriptions[LocalWhiteBoard.Instance.ReturnSkillLevel(skill) - 1];
                break;
            case SkillIndex.secondCommon:
                secondRelicImage.sprite = skill.UiSprite;
                secondRelicDescription.text = skill.SkillsDescriptions[LocalWhiteBoard.Instance.ReturnSkillLevel(skill) - 1];
                break;
            case SkillIndex.legendary:
                legendaryRelicImage.sprite = skill.UiSprite;
                legendaryRelicDescription.text = skill.SkillsDescriptions[LocalWhiteBoard.Instance.ReturnSkillLevel(skill) - 1];
                break;
        }
    }

    public void TurnTotemScreenOff() {
        totemScreen.SetActive(false);
        LocalWhiteBoard.Instance.AnimationOn = false;
    }

    void TurnEquipTabOn() {
        upgradeScreen.SetActive(false);
        equipScreen.SetActive(true);
    }

    void TurnUpgradeTabOn() {
        upgradeScreen.SetActive(true);
        equipScreen.SetActive(false);
    }

    enum ArrowDirection { up, down, left, right }
    void EquipArrowFunction(SkillIndex relicIndex, ArrowDirection arrow) {
        switch (relicIndex) {
            case SkillIndex.firstCommon:
                CommonRelic oldRelicOne = listOfCommonRelics[firstCommonIndex];

                if (arrow == ArrowDirection.right) {
                    firstCommonIndex++;
                    if (firstCommonIndex >= listOfCommonRelics.Count) firstCommonIndex = 0;
                }
                else {
                    firstCommonIndex--;
                    if (firstCommonIndex < 0) firstCommonIndex = listOfCommonRelics.Count - 1;
                }

                if (firstCommonIndex == secondCommonIndex) {
                    secondCommonIndex = listOfCommonRelics.IndexOf(oldRelicOne);
                    DefineImagenAndDescription(oldRelicOne, SkillIndex.secondCommon);
                }

                CommonRelic relicOne = listOfCommonRelics[firstCommonIndex];
                DefineImagenAndDescription(relicOne, SkillIndex.firstCommon);

                LocalWhiteBoard.Instance.EquipRelic(relicOne, 1);
                break;

            case SkillIndex.secondCommon:
                CommonRelic oldRelictwo = listOfCommonRelics[secondCommonIndex];

                if (arrow == ArrowDirection.right) {
                    secondCommonIndex++;
                    if (secondCommonIndex >= listOfCommonRelics.Count) secondCommonIndex = 0;
                }
                else {
                    secondCommonIndex--;
                    if (secondCommonIndex < 0) secondCommonIndex = listOfCommonRelics.Count - 1;
                }

                if (firstCommonIndex == secondCommonIndex) {
                    firstCommonIndex = listOfCommonRelics.IndexOf(oldRelictwo);
                    DefineImagenAndDescription(oldRelictwo, SkillIndex.firstCommon);
                }

                CommonRelic relicTwo = listOfCommonRelics[secondCommonIndex];
                DefineImagenAndDescription(relicTwo, SkillIndex.secondCommon);

                LocalWhiteBoard.Instance.EquipRelic(relicTwo, 2);
                break;

            case SkillIndex.legendary:
                if (arrow == ArrowDirection.right) {
                    legendaryIndex++;
                    if (legendaryIndex >= listOfLegendaryRelics.Count) legendaryIndex = 0;
                }
                else {
                    legendaryIndex--;
                    if (legendaryIndex < 0) legendaryIndex = listOfLegendaryRelics.Count - 1;
                }

                LegendaryRelic legendaryRelic = listOfLegendaryRelics[legendaryIndex];
                DefineImagenAndDescription(legendaryRelic, SkillIndex.legendary);

                LocalWhiteBoard.Instance.EquipRelic(legendaryRelic, 3);
                break;
        }
    }

    #endregion

    #region Update Tab
    void UpdateArrowFunction(ArrowDirection arrow) {
        if (arrow == ArrowDirection.up) {
            updateIndex++;
            if (updateIndex >= listOfCommonRelics.Count) updateIndex = 0;
        }
        else {
            updateIndex--;
            if (updateIndex < 0) updateIndex = listOfCommonRelics.Count - 1;
        }

        relicImage.sprite = listOfCommonRelics[updateIndex].UiSprite;

        ChangeDescriptionPrinceAndImages();
    }

    void ChangeDescriptionPrinceAndImages() {
        int skillLevel = LocalWhiteBoard.Instance.ReturnSkillLevel(listOfCommonRelics[updateIndex]) - 1;

        if (skillLevel < 3) {
            upgradeDescription.text = listOfCommonRelics[updateIndex].UpgradesDescriptions[skillLevel];
            buyPrice.text = prices[skillLevel].ToString();
            buyButton.interactable = LocalWhiteBoard.Instance.ReturnGoldAmount() >= prices[skillLevel];
        }
        else {
            upgradeDescription.text = "Skill no nível máximo!";
            buyPrice.text = "";
            buyButton.interactable = false;
        }

        ChangeUpdatedIcons(skillLevel + 1);
    }

    void ChangeUpdatedIcons(int skillLevel) {
        switch (skillLevel) {
            case 1:
                firstUpgradeImage.color = Color.green;
                secondUpgradeImage.color = Color.red;
                thirdUpgradeImage.color = Color.red;
                fourthUpgradeImage.color = Color.red;
                break;
            case 2:
                firstUpgradeImage.color = Color.green;
                secondUpgradeImage.color = Color.green;
                thirdUpgradeImage.color = Color.red;
                fourthUpgradeImage.color = Color.red;
                break;
            case 3:
                firstUpgradeImage.color = Color.green;
                secondUpgradeImage.color = Color.green;
                thirdUpgradeImage.color = Color.green;
                fourthUpgradeImage.color = Color.red;
                break;
            case 4:
                firstUpgradeImage.color = Color.green;
                secondUpgradeImage.color = Color.green;
                thirdUpgradeImage.color = Color.green;
                fourthUpgradeImage.color = Color.green;
                break;
        }
    }

    void BuyButton() {
        int skillLevel = LocalWhiteBoard.Instance.ReturnSkillLevel(listOfCommonRelics[updateIndex]) - 1;
        LocalWhiteBoard.Instance.RemoveGold(prices[skillLevel]);

        LocalWhiteBoard.Instance.UpdateCommonRelicLevel(listOfCommonRelics[updateIndex], skillLevel + 2);

        ChangeDescriptionPrinceAndImages();
    }



    #endregion
}
