using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffAndDebuffIcon : MonoBehaviour {
    public Image image;
    public TextMeshProUGUI stacksText;
    [SerializeField] RectTransform rectTransform;

    public void UpdateIcon(Sprite sprite, int stacks) {
        try { image.sprite = sprite; }
        catch { }
        if (stacks <= 1) {
            stacksText.text = "";
        }
        else {
            stacksText.text = stacks.ToString();
        }
    }
    private void OnEnable() {
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }
}
