using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AudioVolumeManager : MonoBehaviour {

    private static AudioVolumeManager instance;
    private float musicVolume = 0.75f;
    private float soundEffectVolume = 0.75f;
    private float voiceVolume = 0.75f;
    private string fileName = "volumeData.dat";
    
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

    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
        VolumeData data = new VolumeData(musicVolume, soundEffectVolume, voiceVolume);
        bf.Serialize(file, data);
        file.Close();
    }

    private void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            VolumeData data = (VolumeData) bf.Deserialize(file);
            file.Close();
            musicVolume = data.music;
            soundEffectVolume = data.soundEffect;
            voiceVolume = data.voice;
        }
    }

    private void OnEnable()
    {
        Load();
    }

    private void OnDisable()
    {
        Save();
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

[Serializable]
class VolumeData
{
    public float music;
    public float soundEffect;
    public float voice;

    public VolumeData(float m, float s, float v)
    {
        music = m;
        soundEffect = s;
        voice = v;
    }
}
