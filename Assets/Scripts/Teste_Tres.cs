using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Teste_Tres : NetworkBehaviour {
    [SerializeField] Button[] commonSkillsButtons;
    [SerializeField] Button[] legendarySkillsButtons;
    [SerializeField] Button slotButton, upgradeButton, downgradeButton;
    bool firstSlot = true;

    private void Start() {
        SetSkillsImage(LocalWhiteBoard.Instance.PlayerCharacter);
    }

    void SetSkillsImage(Characters playerCharater) {
        List<Skill> listOfSkills = PlayerSkillConverter.Instance.ReturnCommonSkillList(playerCharater);
        List<Skill> listOfLegendary = PlayerSkillConverter.Instance.ReturnLegendarySkillList(playerCharater);
        for (int i = 0; i < commonSkillsButtons.Length; i++) {
            int index = i;
            commonSkillsButtons[index].GetComponent<Image>().sprite = listOfSkills[index].UiSprite;
            commonSkillsButtons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text  = listOfSkills[index].Name;
            commonSkillsButtons[index].onClick.RemoveAllListeners();
            commonSkillsButtons[index].onClick.AddListener(() => EquipCommonSkill(listOfSkills[index]));
        }
        for (int i = 0; i < legendarySkillsButtons.Length; i++) {
            int index = i;
            legendarySkillsButtons[index].GetComponent<Image>().sprite = listOfLegendary[index].UiSprite;
            legendarySkillsButtons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listOfLegendary[index].Name;
            legendarySkillsButtons[index].onClick.RemoveAllListeners();
            legendarySkillsButtons[index].onClick.AddListener(() => EquipLegendarySkill(listOfLegendary[index]));
        }

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(ChangeSlotButton);

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(UpgradeSkill);

        downgradeButton.onClick.RemoveAllListeners();
        downgradeButton.onClick.AddListener(DowngradeSkill);
    }

    void EquipCommonSkill(Skill skillToEquip) {
        if (firstSlot) {
            Debug.Log(skillToEquip.name);
            LocalWhiteBoard.Instance.AddToCommonDictionary(skillToEquip as CommonRelic);
            LocalWhiteBoard.Instance.EquipRelic(skillToEquip, 1);
        }
        else {
            Debug.Log("SecondSlot");
            LocalWhiteBoard.Instance.AddToCommonDictionary(skillToEquip as CommonRelic);
            LocalWhiteBoard.Instance.EquipRelic(skillToEquip, 2);
        }
    }
    void EquipLegendarySkill(Skill skillToEquip) {
        LocalWhiteBoard.Instance.AddToLegendaryDictionary(skillToEquip as LegendaryRelic);
        LocalWhiteBoard.Instance.EquipRelic(skillToEquip, 3);
    }

    void ChangeSlotButton() {
        firstSlot = !firstSlot;

        if (firstSlot) {
            slotButton.GetComponent<Image>().color = Color.red;
            slotButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
        }
        else {
            slotButton.GetComponent<Image>().color = Color.blue;
            slotButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "2";
        }
    }

    void UpgradeSkill() {
        if (firstSlot){
            Skill currentSkill = LocalWhiteBoard.Instance.ReturnCurrentSkill(1);
            int currentSkillLevel = LocalWhiteBoard.Instance.ReturnCurrentSkillLevel(1);
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(currentSkill as CommonRelic, currentSkillLevel + 1);
        }
        else {
            Skill currentSkill = LocalWhiteBoard.Instance.ReturnCurrentSkill(2);
            int currentSkillLevel = LocalWhiteBoard.Instance.ReturnCurrentSkillLevel(2);
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(currentSkill as CommonRelic, currentSkillLevel + 1);
        }
    }

    void DowngradeSkill() {
        if (firstSlot) {
            Skill currentSkill = LocalWhiteBoard.Instance.ReturnCurrentSkill(1);
            int currentSkillLevel = LocalWhiteBoard.Instance.ReturnCurrentSkillLevel(1);
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(currentSkill as CommonRelic, currentSkillLevel - 1);
        }
        else {
            Skill currentSkill = LocalWhiteBoard.Instance.ReturnCurrentSkill(2);
            int currentSkillLevel = LocalWhiteBoard.Instance.ReturnCurrentSkillLevel(2);
            LocalWhiteBoard.Instance.UpdateCommonRelicLevel(currentSkill as CommonRelic, currentSkillLevel - 1);
        }
    }
}
