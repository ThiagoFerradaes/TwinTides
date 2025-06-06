using FMODUnity;
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
    [SerializeField] EventReference totemOpenSound;

    [Header("Equip Tab")]
    [SerializeField] GameObject equipScreen;
    [SerializeField] Button firstRelicRightArrow, firstRelicLeftArrow, secondRelicRightArrow, secondRelicLeftArrow, legendaryRelicRightArrow, legendaryRelicLeftArrow;
    [SerializeField] TextMeshProUGUI firstRelicDescription, secondRelicDescription, legendaryRelicDescription;
    [SerializeField] Image firstRelicImage, secondRelicImage, legendaryRelicImage;

    [Header("Upgrade Tab")]
    [SerializeField] GameObject upgradeScreen;
    [SerializeField] Button upArrow, downArrow, buyButton;
    [SerializeField] TextMeshProUGUI upgradeDescription, buyPrice;
    [SerializeField]
    Image relicImage, firstUpgradeImage, secondUpgradeImage, thirdUpgradeImage, fourthUpgradeImage, firstLineImage,
        secondLineImage, thirdLineImage, firstRelicNameImage, secondRelicNameImage, legendaryRelicNameImage;
    [SerializeField]
    Sprite firstUpgradeUp, firstUpgradeDown, secondUpgradeUp, secondUpgradeDown, thirdUpgradeUp, thirdUpgradeDown, fourthUpgradeUp, fourthUpgradeDown,
        firstLineUp, firstLineDown, secondLineUp, secondLineDown, thirdLineUp, thirdLineDown;

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

        equipTabButton.gameObject.SetActive(false);
        upgradeTabButton.gameObject.SetActive(true);

        LocalWhiteBoard.Instance.AnimationOn = true;

        if (!totemOpenSound.IsNull) RuntimeManager.PlayOneShot(totemOpenSound);
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
            firstRelicImage.gameObject.SetActive(true);
            DefineImagenAndDescription(firstRelic, SkillIndex.firstCommon);
        }
        else {
            firstRelicImage.gameObject.SetActive(false);
            firstRelicNameImage.gameObject.SetActive(false);
            firstRelicDescription.text = "";
        }


        // Second Relic

        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null) {
            CommonRelic secondRelic = LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo;
            secondCommonIndex = listOfCommonRelics.IndexOf(secondRelic);
            secondRelicImage.gameObject.SetActive(true);
            DefineImagenAndDescription(secondRelic, SkillIndex.secondCommon);
        }
        else{
            secondRelicImage.gameObject.SetActive(false);
            secondRelicNameImage.gameObject.SetActive(false);
            secondRelicDescription.text = "";
        }

        // Legendary Relic

        if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null) {
            LegendaryRelic legendaryRelic = LocalWhiteBoard.Instance.PlayerLegendarySkill;
            legendaryIndex = listOfLegendaryRelics.IndexOf(legendaryRelic);
            legendaryRelicImage.gameObject.SetActive(true);
            DefineImagenAndDescription(legendaryRelic, SkillIndex.legendary);
        }
        else {
            legendaryRelicImage.gameObject.SetActive(false);
            legendaryRelicNameImage.gameObject.SetActive(false);
            legendaryRelicDescription.text = "";
        }

        // Upgrade
        if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
            CommonRelic firstRelic = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne;
            updateIndex = listOfCommonRelics.IndexOf(firstRelic);
            relicImage.gameObject.SetActive(true);
            relicImage.sprite = listOfCommonRelics[updateIndex].UiSprite;
            ChangeDescriptionPrinceAndImages();
        }
        else {
            relicImage.gameObject.SetActive(false);
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
                firstRelicNameImage.sprite = skill.NameSpriteTotem;
                break;
            case SkillIndex.secondCommon:
                secondRelicImage.sprite = skill.UiSprite;
                secondRelicDescription.text = skill.SkillsDescriptions[LocalWhiteBoard.Instance.ReturnSkillLevel(skill) - 1];
                secondRelicNameImage.sprite = skill.NameSpriteTotem;
                break;
            case SkillIndex.legendary:
                legendaryRelicImage.sprite = skill.UiSprite;
                legendaryRelicDescription.text = skill.SkillsDescriptions[LocalWhiteBoard.Instance.ReturnSkillLevel(skill) - 1];
                legendaryRelicNameImage.sprite = skill.NameSpriteTotem;
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
        equipTabButton.gameObject.SetActive(false);
        upgradeTabButton.gameObject.SetActive(true);
    }

    void TurnUpgradeTabOn() {
        upgradeScreen.SetActive(true);
        equipScreen.SetActive(false);
        equipTabButton.gameObject.SetActive(true);
        upgradeTabButton.gameObject.SetActive(false);
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
        var relic = listOfCommonRelics[updateIndex];
        int skillLevel = LocalWhiteBoard.Instance.ReturnSkillLevel(relic) - 1;

        if (skillLevel < 3) {
            upgradeDescription.text = listOfCommonRelics[updateIndex].UpgradesDescriptions[skillLevel];
            buyPrice.text = prices[skillLevel].ToString();
            buyButton.interactable = (LocalWhiteBoard.Instance.ReturnGoldAmount() >= prices[skillLevel] && LocalWhiteBoard.Instance.FragmentsInventory[relic] > 0);
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
                firstUpgradeImage.sprite = firstUpgradeUp;
                secondUpgradeImage.sprite = secondUpgradeDown;
                thirdUpgradeImage.sprite = thirdUpgradeDown;
                fourthUpgradeImage.sprite = fourthUpgradeDown;

                firstLineImage.sprite = firstLineDown;
                secondLineImage.sprite = secondLineDown;
                thirdLineImage.sprite = thirdLineDown;
                break;
            case 2:
                firstUpgradeImage.sprite = firstUpgradeUp;
                secondUpgradeImage.sprite = secondUpgradeUp;
                thirdUpgradeImage.sprite = thirdUpgradeDown;
                fourthUpgradeImage.sprite = fourthUpgradeDown;

                firstLineImage.sprite = firstLineUp;
                secondLineImage.sprite = secondLineDown;
                thirdLineImage.sprite = thirdLineDown;
                break;
            case 3:
                firstUpgradeImage.sprite = firstUpgradeUp;
                secondUpgradeImage.sprite = secondUpgradeUp;
                thirdUpgradeImage.sprite = thirdUpgradeUp;
                fourthUpgradeImage.sprite = fourthUpgradeDown;

                firstLineImage.sprite = firstLineUp;
                secondLineImage.sprite = secondLineUp;
                thirdLineImage.sprite = thirdLineDown;
                break;
            case 4:
                firstUpgradeImage.sprite = firstUpgradeUp;
                secondUpgradeImage.sprite = secondUpgradeUp;
                thirdUpgradeImage.sprite = thirdUpgradeUp;
                fourthUpgradeImage.sprite = fourthUpgradeUp;

                firstLineImage.sprite = firstLineUp;
                secondLineImage.sprite = secondLineUp;
                thirdLineImage.sprite = thirdLineUp;
                break;
        }
    }

    void BuyButton() {
        var relic = listOfCommonRelics[updateIndex];
        int skillLevel = LocalWhiteBoard.Instance.ReturnSkillLevel(relic) - 1;

        LocalWhiteBoard.Instance.RemoveGold(prices[skillLevel]);

        LocalWhiteBoard.Instance.FragmentsInventory[relic]--;

        LocalWhiteBoard.Instance.UpdateCommonRelicLevel(listOfCommonRelics[updateIndex], skillLevel + 2);

        ChangeDescriptionPrinceAndImages();
    }



    #endregion
}
