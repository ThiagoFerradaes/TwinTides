using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : NetworkBehaviour {
    [SerializeField] GameObject loadingScreen;
    [SerializeField] float loadingTime;


    #region Buttons

    public void ExitButton() {
        Application.Quit();
    }
    public void PlayButton() {
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            LoadSceneClientRpc(1);
        }   
    }

    [ClientRpc]
    void LoadSceneClientRpc(int index) {
        StartCoroutine(LoadScene(index));
    }

    IEnumerator LoadScene(int index) {
        loadingScreen.SetActive(true);


        yield return new WaitForSecondsRealtime(loadingTime);

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
    }

    #endregion
}
