using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsUI : MonoBehaviour {
    [SerializeField] Image iconPrefab;

    HealthManager _characterHealthManager;
    readonly Dictionary<Buff, Image> activeBuffsList = new();
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
        _characterHealthManager.OnBuffAdded += AddBuff;
        _characterHealthManager.OnBuffRemoved += RemoveBuff;
    }
    void AddBuff(Buff buffAdded, int stacks) {
        if (activeBuffsList.ContainsKey(buffAdded)) {
            activeBuffsList[buffAdded].GetComponent<BuffAndDebuffIcon>().UpdateIcon(buffAdded.BuffColor, stacks);   
        }
        else {
            Image icon = GetIconFromPooling();
            activeBuffsList.Add(buffAdded, icon);
            icon.GetComponent<BuffAndDebuffIcon>().UpdateIcon(buffAdded.BuffColor, stacks);
            icon.gameObject.SetActive(true);
        }
    }

    void RemoveBuff(Buff buffRemoved, int stacks) {
        if (activeBuffsList.ContainsKey(buffRemoved)) {
            activeBuffsList[buffRemoved].gameObject.SetActive(false);
            activeBuffsList.Remove(buffRemoved);
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
