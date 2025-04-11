using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChestManager: NetworkBehaviour
{
    public static ChestManager Instance;

    [SerializeField] float cooldownCommonChestRespawn;
    int amountOfMediumChestOpened;

    public void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

    }

    public void Start() {
        Chest.StatCommonChestCooldown += Chest_StatCommonChestCooldown;
        Chest.MediumChestOpened += Chest_MediumChestOpened;
    }

    private void Chest_MediumChestOpened(object sender, System.EventArgs e) {
        amountOfMediumChestOpened++;
    }

    private void Chest_StatCommonChestCooldown(object sender, Chest.ChestEventArgs e) {
        StartCoroutine(CommonChestCooldown(e.chest));
    }

    IEnumerator CommonChestCooldown(Chest chest) {
        yield return new WaitForSeconds(cooldownCommonChestRespawn);
        chest.gameObject.SetActive(true);
    }

    public int ReturnAmountOfMediumChestOpened() {
        return amountOfMediumChestOpened;
    }

    private void OnDisable() {
        Chest.StatCommonChestCooldown -= Chest_StatCommonChestCooldown;
        Chest.MediumChestOpened -= Chest_MediumChestOpened;
    }
}
