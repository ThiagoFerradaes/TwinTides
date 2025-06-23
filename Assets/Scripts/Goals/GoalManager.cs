using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Mission {
    public string name;
    public Sprite sprite;
    public bool isCompleted;
}
public class GoalManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image goalsImage;

    [Header("Missões")]
    [SerializeField] private List<Mission> missions;

    private int currentMissionIndex = 0;

    private void Start() {
        Camps.OnAllEnemiesDeadStatic += OnZombiesDefeated;
        Chest.OnFindRelics += OnOpenChest;
        Chest.OnKeyObtain += OnKeyObtain;
        KeyLockManager.OnAllKeysUsed += AllKeysUsedEvent;
        Camps.OnBlackBeardFound += OnFoundBlackBeard;
        TotemManager.OnUpgradeRelic += OnUpgrade;
        UpdateGoalImage();
    }


    private void OnDestroy() {
        try {
            Camps.OnAllEnemiesDeadStatic -= OnZombiesDefeated;
            Chest.OnFindRelics -= OnOpenChest;
            Chest.OnKeyObtain -= OnKeyObtain;
            KeyLockManager.OnAllKeysUsed -= AllKeysUsedEvent;
            Camps.OnBlackBeardFound -= OnFoundBlackBeard;
            TotemManager.OnUpgradeRelic -= OnUpgrade;
        }
        catch { }
    }
    private void OnZombiesDefeated() {
        if (missions[currentMissionIndex].name != "FightZombies") return;

        CompleteCurrentMission();
    }
    private void OnOpenChest() {
        if (missions[currentMissionIndex].name != "OpenChest") return;

        CompleteCurrentMission();
    }
    private void OnUpgrade() {
        if (missions[currentMissionIndex].name != "UpgradeRelic") return;

        CompleteCurrentMission();
    }
    private void OnKeyObtain() {
        if (missions[currentMissionIndex].name != "FindKey") return;

        CompleteCurrentMission();
    }
    private void AllKeysUsedEvent() {
        if (missions[currentMissionIndex].name != "UseKeys") return;

        CompleteCurrentMission();
    }

    private void OnFoundBlackBeard() {
        if (missions[currentMissionIndex].name != "FindBlackBeard") return;

        CompleteCurrentMission();
    }
    public void CompleteCurrentMission() {
        if (currentMissionIndex < missions.Count) {
            missions[currentMissionIndex].isCompleted = true;
            currentMissionIndex++;
            UpdateGoalImage();
        }
    }

    private void UpdateGoalImage() {
        if (currentMissionIndex < missions.Count) {
            goalsImage.sprite = missions[currentMissionIndex].sprite;
        }
        else {
            goalsImage.gameObject.SetActive(false); 
        }
    }
}
