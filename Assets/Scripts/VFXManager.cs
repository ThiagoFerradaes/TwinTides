using UnityEngine;

public class VFXManager : MonoBehaviour
{
   [SerializeField] GameObject pathToBlackBeard;
    void Start()
    {
        KeyLockManager.OnAllKeysUsed += KeyLockManager_OnAllKeysUsed;
    }

    private void OnDestroy() {
        try {
            KeyLockManager.OnAllKeysUsed -= KeyLockManager_OnAllKeysUsed;
        }
        catch { }
    }
    private void KeyLockManager_OnAllKeysUsed() {
        pathToBlackBeard.SetActive(true);
    }
}
