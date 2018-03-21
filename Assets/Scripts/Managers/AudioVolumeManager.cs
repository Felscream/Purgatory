using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeManager : MonoBehaviour {

    private static AudioVolumeManager instance;
    private float musicVolume = 0.75f;
    private float voiceVolume = 0.75f;
    private float soundEffectVolume = 0.75f;

    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //we want to keep it the whole time the game is running
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static AudioVolumeManager GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(AudioVolumeManager));
            return null;
        }
        return instance;
    }

    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            MusicVolume = value;
        }
    }

    public float SoundEffectVolume
    {
        get
        {
            return soundEffectVolume;
        }
        set
        {
            soundEffectVolume = value;
        }
    }

    public float VoiceVolume
    {
        get
        {
            return voiceVolume;
        }
        set
        {
            voiceVolume = value;
        }
    }
}
