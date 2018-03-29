using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour {

    AudioVolumeManager audioVolumeManager;
    // Use this for initialization
    void Start () {
        audioVolumeManager = AudioVolumeManager.GetInstance();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLobbyScene()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void QuitGame()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        Debug.Log("Quit!");
        Application.Quit();
    }
}
