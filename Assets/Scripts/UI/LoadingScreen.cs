using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] GameObject thingImage;
    [SerializeField] float thingRotateSpeed;
    [SerializeField] Image insideBar;

    public void Activate(float amountOfTimeToLoad) {

        insideBar.fillAmount = 0;

        thingImage.transform.rotation = Quaternion.identity;

        gameObject.SetActive(true);

        StopAllCoroutines();

        StartCoroutine(LoadinScreenEffectsRoutine(amountOfTimeToLoad));
    }
    IEnumerator LoadinScreenEffectsRoutine(float timeToLoad) {

        float timer = 0;

        while (true) {

            thingImage.transform.Rotate(0, 0, -thingRotateSpeed * Time.deltaTime);

            if (insideBar.fillAmount < 1f) {
                timer += Time.deltaTime;
                insideBar.fillAmount = Mathf.Clamp01(timer / timeToLoad);
            }

            yield return null;
        }
    }
}
