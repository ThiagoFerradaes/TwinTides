using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuffsUI : MonoBehaviour
{
    [SerializeField] Image iconPrefab;
    HealthManager _characterHealthManager;
    readonly Dictionary<Debuff, Image> activeDebuffsList = new();
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
        _characterHealthManager.OnDeath += ClearDebuffs;
    }

    void ClearDebuffs() {
        foreach (var debuff in activeDebuffsList.Values) debuff.gameObject.SetActive(false);
        activeDebuffsList.Clear();
    }
    void AddDebuff(Debuff debuffAdded, int stacks) {
        if (activeDebuffsList.ContainsKey(debuffAdded)) {
            activeDebuffsList[debuffAdded].GetComponent<BuffAndDebuffIcon>().UpdateIcon(debuffAdded.debuffSprite, stacks);
        }
        else {
            Image icon = GetIconFromPooling();
            activeDebuffsList.Add(debuffAdded, icon);
            icon.GetComponent<BuffAndDebuffIcon>().UpdateIcon(debuffAdded.debuffSprite, stacks);
            icon.gameObject.SetActive(true);
        }
    }

    void RemoveDebuff(Debuff debuffRemoved, int stacks) {
        if (activeDebuffsList.ContainsKey(debuffRemoved)) {
            activeDebuffsList[debuffRemoved].gameObject.SetActive(false);
            activeDebuffsList.Remove(debuffRemoved);
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
