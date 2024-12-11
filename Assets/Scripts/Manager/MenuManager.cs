using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [SerializeField] GameObject loadingScreen;
    [SerializeField] float loadingTime;
    [SerializeField] NetworkManager networkManager;


    #region Buttons

    public void ExitButton() {
        Application.Quit();
    }
    public void PlayButton() {
        if (networkManager.ConnectedClientsList.Count == 2)
        {
            StartCoroutine(LoadScene(1));
        }   
    }

    IEnumerator LoadScene(int index) {

        loadingScreen.SetActive(true);


        yield return new WaitForSecondsRealtime(loadingTime);

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
    }
    #endregion
}
