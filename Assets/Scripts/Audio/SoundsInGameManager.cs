using FMODUnity;
using UnityEngine;

public class SoundsInGameManager : MonoBehaviour
{
    [SerializeField] EventReference hoverSound;
    [SerializeField] EventReference clickSound;
    [SerializeField] EventReference hoverSoundTotem;
    [SerializeField] EventReference clickSoundTotem;
    [SerializeField] EventReference changeRelicSound;
    [SerializeField] EventReference exitTotemSound;
    [SerializeField] EventReference switchTotemTabSound;
    [SerializeField] EventReference upgradeRelicSound;

    public void HoverSound(bool isTotem) {
        if (isTotem && !hoverSoundTotem.IsNull) RuntimeManager.PlayOneShot(hoverSoundTotem);
        else if (!isTotem && !hoverSound.IsNull) RuntimeManager.PlayOneShot(hoverSound);
    }
    public void ClickSound() {
        if (!clickSound.IsNull) RuntimeManager.PlayOneShot(clickSound);
    }

    public void ChangeRelic() {
        if (!changeRelicSound.IsNull) RuntimeManager.PlayOneShot(changeRelicSound);
    }

    public void ExitTotem() {
        if (!exitTotemSound.IsNull) RuntimeManager.PlayOneShot(exitTotemSound);
    }

    public void UpgradeRelic() {
        if (!upgradeRelicSound.IsNull) RuntimeManager.PlayOneShot(upgradeRelicSound);
    }
    public void SwitchTotemTab() {
        if (!switchTotemTabSound.IsNull) RuntimeManager.PlayOneShot(switchTotemTabSound);
    }
}
