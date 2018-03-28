using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour {

    private static AudioVolumeManager instance;
    private float musicVolume = 0.75f;
    private float soundEffectVolume = 0.75f;
    private float voiceVolume = 0.75f;
    private string fileName = "volumeData.dat";

    [SerializeField] private Sound[] themes;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

    public void PlayTheme(string name)
    {
        Sound t = Array.Find(themes, theme => theme.name == name);
        if(t == null)
        {
            Debug.Log("Theme " + name + " not found");
            return;
        }
        foreach(Sound s in themes)
        {
            s.source.Stop();
        }
        t.source.Play();
    }
    
    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
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
        foreach (Sound m in themes)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.pitch = m.pitch;
            m.source.volume = musicVolume;
            m.source.loop = m.loop;
            m.source.spatialBlend = 0.0f;
        }
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
            musicVolume = value;
            foreach (Sound m in themes)
            {
                m.source.volume = musicVolume;
            }
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
