using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    private static SceneController instance;
    [SerializeField] string loadingSceneName = "Loading_Scene";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static SceneController GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(SceneController));
            return null;
        }
        return instance;
    }

    public IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation loadingScreen = SceneManager.LoadSceneAsync(loadingSceneName);
        while (!loadingScreen.isDone)
        {
            yield return null;
        }
        Time.timeScale = 1.0f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Time.timeScale = 1.0f; //just to be sure
    }
}
