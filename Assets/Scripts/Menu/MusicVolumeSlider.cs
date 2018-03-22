using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour {

    AudioVolumeManager volumeManager;
    Slider musicVolumeSlider;

    // Use this for initialization
    void Start () {
        volumeManager = AudioVolumeManager.GetInstance();
        musicVolumeSlider = GetComponent<Slider>();

        //Adds a listener to the main input field and invokes a method when the value changes.
        musicVolumeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }
    
    public void ValueChangeCheck()
    {
        Debug.Log("Je passe ici");
        //Displays the value of the slider in the console.
        volumeManager.MusicVolume = musicVolumeSlider.value;
        
    }
}
