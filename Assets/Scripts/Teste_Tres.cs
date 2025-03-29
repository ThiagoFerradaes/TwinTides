using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Teste_Tres : NetworkBehaviour {
    [SerializeField] Button[] commonSkillsButtons;
    [SerializeField] Button[] legendarySkillsButtons;
    [SerializeField] Button slotButton;
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
            commonSkillsButtons[index].onClick.RemoveAllListeners();
            commonSkillsButtons[index].onClick.AddListener(() => EquipCommonSkill(listOfSkills[index]));
        }
        for (int i = 0; i < legendarySkillsButtons.Length; i++) {
            int index = i;
            legendarySkillsButtons[index].GetComponent<Image>().sprite = listOfLegendary[index].UiSprite;
            legendarySkillsButtons[index].onClick.RemoveAllListeners();
            legendarySkillsButtons[index].onClick.AddListener(() => EquipLegendarySkill(listOfLegendary[index]));
        }

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(ChangeSlotButton);
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

        if (firstSlot) slotButton.GetComponent<Image>().color = Color.red;
        else slotButton.GetComponent<Image>().color = Color.blue;
    }
}
