using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float timeBetweenDots;

    private void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(DotsCoroutine());
    }
    IEnumerator DotsCoroutine() {
        while (true) {
            loadingText.text = "Loading";
            yield return new WaitForSeconds(timeBetweenDots);
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(timeBetweenDots);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(timeBetweenDots);
            loadingText.text = "Loading...";
            yield return new WaitForSeconds(timeBetweenDots);
        }
    }
}
