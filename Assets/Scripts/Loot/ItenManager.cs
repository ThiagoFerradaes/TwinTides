using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItenManager : MonoBehaviour {
    public static ItenManager Instance;
    [SerializeField] RectTransform goldScreen, fragmentsScreen, keyScreen, legendaryScreen;
    [SerializeField] Vector3 inicialScreenPosition, finalScreenPosition;
    [SerializeField] float fadeOutDuration, timeBetweenEachText;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void TurnScreenOn(CommonRelic relic, float gold, int amountOfKeys, LegendaryRelic legendaryRelic) {
        StartCoroutine(DisplayItens(relic, gold, amountOfKeys, legendaryRelic));
    }

    IEnumerator DisplayItens(CommonRelic relic, float gold, int amountOfKeys, LegendaryRelic legendaryRelic) {

        if (gold != 0) {
            goldScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gold.ToString("F0");
            StartCoroutine(ShowFloatingUI(goldScreen));
            yield return new WaitForSeconds(timeBetweenEachText);
        }

        if (relic != null) {
            fragmentsScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = relic.Name;
            fragmentsScreen.transform.GetChild(1).GetComponent<Image>().sprite = relic.UiSprite;
            StartCoroutine(ShowFloatingUI(fragmentsScreen));
            yield return new WaitForSeconds(timeBetweenEachText);
        }

        if (amountOfKeys != 0) {
            keyScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = amountOfKeys.ToString("F0");
            StartCoroutine(ShowFloatingUI(keyScreen));
            yield return new WaitForSeconds(timeBetweenEachText);
        }

        if (legendaryRelic != null) {
            legendaryScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = legendaryRelic.Name;
            legendaryScreen.transform.GetChild(1).GetComponent<Image>().sprite = legendaryRelic.UiSprite;
            StartCoroutine(ShowFloatingUI(legendaryScreen));
        }
    }

    IEnumerator ShowFloatingUI(RectTransform ui) {
        ResetAlpha(ui);
        ui.anchoredPosition = inicialScreenPosition;
        ui.gameObject.SetActive(true);

        FadeOutUI(ui);

        Vector2 start = inicialScreenPosition;
        Vector2 end = finalScreenPosition;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration) {
            ui.anchoredPosition = Vector2.Lerp(start, end, elapsed / fadeOutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ui.anchoredPosition = end;
    }

    void FadeOutUI(RectTransform uiTransform) {
        foreach (var image in uiTransform.GetComponentsInChildren<Image>()) {
            image.DOFade(0f, fadeOutDuration);
        }

        foreach (var text in uiTransform.GetComponentsInChildren<TextMeshProUGUI>()) {
            text.DOFade(0f, fadeOutDuration);
        }

        DOVirtual.DelayedCall(fadeOutDuration, () => uiTransform.gameObject.SetActive(false));
    }

    void ResetAlpha(RectTransform uiTransform) {
        foreach (var image in uiTransform.GetComponentsInChildren<Image>()) {
            var color = image.color;
            color.a = 1f;
            image.color = color;
        }

        foreach (var text in uiTransform.GetComponentsInChildren<TextMeshProUGUI>()) {
            var color = text.color;
            color.a = 1f;
            text.color = color;
        }
    }
}
