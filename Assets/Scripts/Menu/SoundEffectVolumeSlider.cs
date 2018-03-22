using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectVolumeSlider : MonoBehaviour
{

    AudioVolumeManager volumeManager;
    Slider soundEffectVolumeSlider;

    // Use this for initialization
    void Start()
    {
        volumeManager = AudioVolumeManager.GetInstance();
        soundEffectVolumeSlider = GetComponent<Slider>();
    }
    
    public void ValueChangeCheck()
    {
        //Displays the value of the slider in the console.
        volumeManager.SoundEffectVolume = soundEffectVolumeSlider.value;
        Debug.Log(volumeManager.SoundEffectVolume);
    }
}