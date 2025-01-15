using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuffsUI : MonoBehaviour
{
    [SerializeField] Image iconPrefab;
    HealthManager _characterHealthManager;
    readonly Dictionary<Debuff, Image> activeBuffsList = new();
    readonly List<Image> listOfIcons = new();

    private void Start() {
        CreatePooling();
    }
    private void OnEnable() {
        PlayerSetUp.OnPlayerSpawned += SetUpHealthManager;
    }
    private void OnDisable() {
        PlayerSetUp.OnPlayerSpawned -= SetUpHealthManager;
    }
    void SetUpHealthManager(GameObject player) {
        _characterHealthManager = player.GetComponent<HealthManager>();
        _characterHealthManager.OnDebuffAdded += AddDebuff;
        _characterHealthManager.OnDebuffRemoved += RemoveDebuff;
    }
    void AddDebuff(Debuff debuffAdded, int stacks) {
        if (activeBuffsList.ContainsKey(debuffAdded)) {
            activeBuffsList[debuffAdded].GetComponent<BuffAndDebuffIcon>().UpdateIcon(debuffAdded.DebuffColor, stacks);
        }
        else {
            Image icon = GetIconFromPooling();
            activeBuffsList.Add(debuffAdded, icon);
            icon.GetComponent<BuffAndDebuffIcon>().UpdateIcon(debuffAdded.DebuffColor, stacks);
            icon.gameObject.SetActive(true);
        }
    }

    void RemoveDebuff(Debuff debuffRemoved, int stacks) {
        if (activeBuffsList.ContainsKey(debuffRemoved)) {
            activeBuffsList[debuffRemoved].gameObject.SetActive(false);
            activeBuffsList.Remove(debuffRemoved);
        };
    }

    void CreatePooling() {
        for (int i = 0; i < 10; i++) {
            Image icon = Instantiate(iconPrefab);
            icon.gameObject.SetActive(false);
            icon.transform.SetParent(transform);
            listOfIcons.Add(icon);
        }
    }

    Image GetIconFromPooling() {
        for (int i = 0; i < listOfIcons.Count; i++) {
            if (!listOfIcons[i].gameObject.activeInHierarchy) {
                return listOfIcons[i];
            }
        }

        Image icon = Instantiate(iconPrefab);
        icon.gameObject.SetActive(false);
        icon.transform.SetParent(transform);
        listOfIcons.Add(icon);
        return icon;
    }
}
