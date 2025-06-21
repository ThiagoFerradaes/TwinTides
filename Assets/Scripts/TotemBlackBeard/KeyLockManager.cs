using System;
using UnityEngine;

public class KeyLockManager : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] Transform finalDoorPosition;

    int totensUnlocked;

    public static event Action OnAllKeysUsed;

    private void Start() {
        KeyLockTotem.OnHasAllKeys += TotemHasKeysEvent;
    }

    private void TotemHasKeysEvent() {
        totensUnlocked++;

        if (totensUnlocked == 2) MoveDoor();
    }

    void MoveDoor() {
        OnAllKeysUsed?.Invoke();
        door.transform.position = finalDoorPosition.position;
    }
}
