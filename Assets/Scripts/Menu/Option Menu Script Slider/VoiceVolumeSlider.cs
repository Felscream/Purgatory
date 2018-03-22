using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceVolumeSlider : MonoBehaviour
{

    AudioVolumeManager volumeManager;
    Slider voiceVolumeSlider;

    // Use this for initialization
    void Start()
    {
        volumeManager = AudioVolumeManager.GetInstance();
        voiceVolumeSlider = GetComponent<Slider>();

        // Initialize
        voiceVolumeSlider.value = volumeManager.VoiceVolume;

        //Adds a listener to the main input field and invokes a method when the value changes.
        voiceVolumeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }
    
    public void ValueChangeCheck()
    {
        //Displays the value of the slider in the console.
        volumeManager.VoiceVolume = voiceVolumeSlider.value;
        Debug.Log(volumeManager.VoiceVolume);
    }
}