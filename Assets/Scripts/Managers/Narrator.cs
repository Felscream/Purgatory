using System.Collections;
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
    [SerializeField] protected AudioClip[] knightUltimate;
    [SerializeField] protected AudioClip[] sorcererUltimate;
    [SerializeField] protected AudioClip[] archerUltimate;
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

    protected float PlayRandom(AudioClip[] comments)
    {
        int i = 0;
        if (comments.Length>0)
        {
            i = Random.Range(0, comments.Length);
            audioSource.clip = comments[i]; //To get the current clip is needed from an exterior component
            audioSource.Play();
            lastCommentTime = Time.time;
        }
        return comments[i].length;
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

    public float Ultimate(Enum_Champion champ)
    {
        float length = 0.0f;
        switch (champ)
        {
            case Enum_Champion.Knight:
                length = PlayRandom(knightUltimate);
                break;
            case Enum_Champion.Sorcerer:
                length = PlayRandom(sorcererUltimate);
                break;
            case Enum_Champion.Archer:
                length = PlayRandom(archerUltimate);
                break;
        }
        return length;
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
