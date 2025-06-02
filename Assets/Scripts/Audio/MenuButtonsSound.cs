using FMODUnity;
using UnityEngine;

public class MenuButtonsSound : MonoBehaviour
{
    [SerializeField] EventReference hoverSound;
    [SerializeField] EventReference clickSound;
    
    public void ClickSound() {
        RuntimeManager.PlayOneShot(clickSound);
    }

    public void HoverSound() {
        RuntimeManager.PlayOneShot(hoverSound);
    }
}
