using TMPro;
using UnityEngine;

public class KeyCounterManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI amountOfKeysText;


    private void Start() {
        LocalWhiteBoard.OnAddKey += LocalWhiteBoard_OnAddKey;
    }

    private void LocalWhiteBoard_OnAddKey(int obj) {
        amountOfKeysText.text = $" {obj} / 3";
    }
}
