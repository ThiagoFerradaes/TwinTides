using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItenManager : MonoBehaviour {
    public static ItenManager Instance;
    [SerializeField] RectTransform allScreen, goldScreen, fragmentsScreen, keyScreen, legendaryScreen;
    [SerializeField] float movementInY;
    [SerializeField] float fadeOutDuration, textDuration;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void TurnScreenOn(CommonRelic relic, float gold, int amountOfKeys, LegendaryRelic legendaryRelic) {
        StartCoroutine(DisplayItens(relic, gold, amountOfKeys, legendaryRelic));
    }

    IEnumerator DisplayItens(CommonRelic relic, float gold, int amountOfKeys, LegendaryRelic legendaryRelic) {
        allScreen.gameObject.SetActive(true);

        if (gold != 0) {
            goldScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gold.ToString("F0");
            goldScreen.gameObject.SetActive(true);
        }

        if (relic != null) {
            fragmentsScreen.transform.GetChild(1).GetComponent<Image>().sprite = relic.UiSprite;
            fragmentsScreen.gameObject.SetActive(true);
        }

        if (amountOfKeys != 0) {
            keyScreen.gameObject.SetActive(true);
        }

        if (legendaryRelic != null) {
            legendaryScreen.transform.GetChild(1).GetComponent<Image>().sprite = legendaryRelic.UiSprite;
            legendaryScreen.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(textDuration);

        FadeOutUI();
    }

    void FadeOutUI() {
        CanvasGroup canvasGroup = allScreen.GetComponent<CanvasGroup>();
        RectTransform rect = allScreen.GetComponent<RectTransform>();

        // Movimento para cima (slide)
        rect.DOAnchorPosY(rect.anchoredPosition.y + movementInY, fadeOutDuration).SetEase(Ease.InOutSine);

        // Fade out
        canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            allScreen.gameObject.SetActive(false);

            // Resetar para próxima vez
            canvasGroup.alpha = 1f;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - movementInY);

            // Esconde os elementos internos também
            goldScreen.gameObject.SetActive(false);
            fragmentsScreen.gameObject.SetActive(false);
            keyScreen.gameObject.SetActive(false);
            legendaryScreen.gameObject.SetActive(false);
        });
    }
}
