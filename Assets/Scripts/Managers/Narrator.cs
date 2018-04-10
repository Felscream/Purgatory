﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrator : MonoBehaviour {

    [Header("Duration between commentary")]
    [SerializeField] protected float minTime = 5;
    [SerializeField] protected float maxTime = 30;

    [Header("Comments")]
    [SerializeField] protected AudioClip[] startComments;
    [SerializeField] protected AudioClip[] attackComments;
    [SerializeField] protected AudioClip[] clashComments;
    [SerializeField] protected AudioClip[] ultimateComments;
    [SerializeField] protected AudioClip[] guardComments;
    [SerializeField] protected AudioClip[] parryComments;
    [SerializeField] protected AudioClip[] deathComments;
    [SerializeField] protected AudioClip[] endComments;
    [SerializeField] protected AudioClip[] randomComments;

    protected float lastCommentTime;
    protected AudioSource audioSource;
    private AudioVolumeManager audioVolumeManager;
    private static Narrator instance;

    protected float TimePassed
    {
        get
        {
            return Time.time - lastCommentTime;
        }
    }
    protected bool WillComment
    {
        get
        {
            float i;

            // Not enough time have passed
            if (minTime > TimePassed) return false;

            // Randomly choose if a comment will occur (the more time passed the higher the chance of happening)
            i = Random.Range(0f, (maxTime - minTime));
            if (i < TimePassed) return true;

            return false;
        }
    }

    public static Narrator Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("No instance of " + typeof(Narrator));
                return null;
            }
            return instance;
        }

    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        lastCommentTime = 0;
        audioVolumeManager = AudioVolumeManager.GetInstance();
        ManagerInGame.GetInstance().AddAudioSource(audioSource);
    }

	// Update is called once per frame
	void Update () {
        if(TimePassed >= maxTime && !ManagerInGame.GetInstance().EndGame)
        {
            PlayRandom(randomComments);
        }
        if(audioSource.volume != audioVolumeManager.VoiceVolume)
        {
            audioSource.volume = audioVolumeManager.VoiceVolume;
        }
    }

    protected void PlayRandom(AudioClip[] comments)
    {
        if(comments.Length>0)
        {
            int i = Random.Range(0, comments.Length);
            audioSource.clip = comments[i]; //To get the current clip is needed from an exterior component
            audioSource.Play();
            lastCommentTime = Time.time;
        }
    }

    protected IEnumerator PlayNext(AudioClip[] comments)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        PlayRandom(comments);
    }

    public void StartOfTheGame()
    {
        PlayRandom(startComments);
    }

    public void Attack()
    {
        if ( WillComment )
            PlayRandom(attackComments);
    }

    public void Clash()
    {
        if (WillComment)
            PlayRandom(clashComments);
    }

    public void Ultimate()
    {
        if (WillComment)
            PlayRandom(ultimateComments);
    }

    public void Guard()
    {
        if (WillComment)
            PlayRandom(guardComments);
    }

    public void Parry()
    {
        if (WillComment)
            PlayRandom(parryComments);
    }

    public void Death()
    {
        StartCoroutine(PlayNext(deathComments));
    }

    public void End()
    {
        audioSource.Stop();
        PlayRandom(endComments);
    }

    public AudioSource AudioSource
    {
        get
        {
            return audioSource;
        }
    }
}
