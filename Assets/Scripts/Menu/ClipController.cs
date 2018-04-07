using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ClipController : MonoBehaviour {

    VideoPlayer clipStart;
    VideoPlayer clipOption;
    VideoPlayer clipBackToMenu;
    VideoPlayer clipControlMenu;
    VideoPlayer clipBackToOptionMenu;
    VideoPlayer clipCreditMenu;
    VideoPlayer clipBackFromCreditToOptionMenu;

    public GameObject LaunchMainMenu;
    public GameObject LaunchOptionsClip;
    public GameObject LaunchBackToMenu;
    public GameObject LaunchControlMenu;
    public GameObject LaunchBackToOptionMenu;
    public GameObject LaunchCreditMenu;
    public GameObject LaunchBackFromCreditToOptionMenu;

    public RectTransform MainMenu;
    public RectTransform OptionsMenu;
    public RectTransform ControlMenu;
    public RectTransform CreditMenu;

    private AudioVolumeManager audioVolumeManager;
    
    private void Awake()
    {
        
        MainMenu.gameObject.SetActive(false);
        OptionsMenu.gameObject.SetActive(false);
        ControlMenu.gameObject.SetActive(false);
        CreditMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(true);

        ClipStart = LaunchMainMenu.GetComponent<VideoPlayer>();
        ClipOption = LaunchOptionsClip.GetComponent<VideoPlayer>();
        ClipBackToMenu = LaunchBackToMenu.GetComponent<VideoPlayer>();
        ClipControlMenu = LaunchControlMenu.GetComponent<VideoPlayer>();
        ClipBackToOptionMenu = LaunchBackToOptionMenu.GetComponent<VideoPlayer>();
        ClipCreditMenu = LaunchCreditMenu.GetComponent<VideoPlayer>();
        ClipBackFromCreditToOptionMenu = LaunchBackFromCreditToOptionMenu.GetComponent<VideoPlayer>();
    }

    // Use this for initialization
    void Start()
    {
        audioVolumeManager = AudioVolumeManager.GetInstance();
        audioVolumeManager.PlayTheme("MainMenuTheme");
        //MainMenu.gameObject.SetActive(false);
        PlayMainMenuFirstTime();
    }

    // Update is called once per frame
    void Update () {


        if (!ClipStart.isPlaying && ClipStart.time >= 1.0f)
        {
            ClipStart.targetCameraAlpha = 0.4f;
            LaunchCanvasMainMenu();
        }

        if (!ClipOption.isPlaying && ClipOption.time >= 1.0f)
        {
            ClipOption.targetCameraAlpha = 0.4f;
            ClipBackToMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasOptionsMenu();
        }

        if (!ClipBackToMenu.isPlaying && ClipBackToMenu.time >= 1.0f)
        {
            ClipOption.targetCameraAlpha = 1.0f;
            ClipBackToMenu.targetCameraAlpha = 0.4f;
            LaunchCanvasMainMenu();
        }

        if (!ClipControlMenu.isPlaying && ClipControlMenu.time >= 0.5f)
        {
            ClipControlMenu.targetCameraAlpha = 0.0f;
            ClipBackToOptionMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasControlMenu();
        }

        if (!ClipCreditMenu.isPlaying && ClipCreditMenu.time >= 0.5f)
        {
            ClipCreditMenu.targetCameraAlpha = 0.0f;
            ClipBackToOptionMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasCreditMenu();
        }

        if (!ClipBackToOptionMenu.isPlaying && ClipBackToOptionMenu.time >= 0.5f)
        {
            ClipControlMenu.targetCameraAlpha = 1.0f;
            ClipBackToOptionMenu.targetCameraAlpha = 0.4f;
            LaunchCanvasOptionsMenu();
        }

        if (!ClipBackFromCreditToOptionMenu.isPlaying && ClipBackFromCreditToOptionMenu.time >= 0.5f)
        {
            ClipCreditMenu.targetCameraAlpha = 1.0f;
            ClipBackFromCreditToOptionMenu.targetCameraAlpha = 0.4f;
            LaunchCanvasOptionsMenu();
        }
    }

    public void ChangeClipMainMenuToOptionsMenu()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        MainMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchControlMenu.gameObject.SetActive(false);
        LaunchBackToOptionMenu.gameObject.SetActive(false);

        LaunchOptionsClip.SetActive(true);

        PlayOptionsMenu();
    }

    public void ChangeClipOptionsMenuToControlMenu()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        OptionsMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchOptionsClip.gameObject.SetActive(false);
        LaunchBackToOptionMenu.gameObject.SetActive(false);
        LaunchCreditMenu.gameObject.SetActive(false);

        LaunchControlMenu.SetActive(true);

        PlayControlMenu();
    }

    //
    public void ChangeClipOptionsMenuToCreditMenu()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        OptionsMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchOptionsClip.gameObject.SetActive(false);
        LaunchBackToOptionMenu.gameObject.SetActive(false);
        LaunchControlMenu.gameObject.SetActive(false);

        LaunchCreditMenu.SetActive(true);

        PlayCreditMenu();
    }

    public void ChangeClipBackToMenu()
    {
        audioVolumeManager.PlaySoundEffect("Cancel");
        OptionsMenu.gameObject.SetActive(false);
        
        LaunchMainMenu.gameObject.SetActive(false);
        LaunchControlMenu.gameObject.SetActive(false);
        LaunchOptionsClip.gameObject.SetActive(false);
        LaunchBackToOptionMenu.gameObject.SetActive(false);

        LaunchBackToMenu.SetActive(true);

        PlayBackToMenu();
    }

    public void ChangeClipControlBackToOptionMenu()
    {
        audioVolumeManager.PlaySoundEffect("Cancel");
        ControlMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchControlMenu.gameObject.SetActive(false);
        LaunchOptionsClip.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchCreditMenu.gameObject.SetActive(false);
        LaunchBackFromCreditToOptionMenu.SetActive(false);

        LaunchBackToOptionMenu.SetActive(true);

        PlayBackToOptionMenu();
    }

    public void ChangeClipControlBackFromCreditToOptionMenu()
    {
        audioVolumeManager.PlaySoundEffect("Cancel");
        CreditMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchControlMenu.gameObject.SetActive(false);
        LaunchOptionsClip.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchCreditMenu.gameObject.SetActive(false);
        LaunchBackToOptionMenu.SetActive(false);

        LaunchBackFromCreditToOptionMenu.SetActive(true);

        PlayBackFromCreditToOptionMenu();
    }

    private void PlayBackFromCreditToOptionMenu()
    {
        ClipBackFromCreditToOptionMenu.Play();
    }

    void PlayBackToMenu()
    {
        ClipBackToMenu.Play();
    }

    void PlayOptionsMenu()
    {
        ClipOption.Play();
    }

    private void PlayMainMenuFirstTime()
    {
        ClipStart.Play();
    }

    private void PlayControlMenu()
    {
        ClipControlMenu.Play();
    }

    private void PlayCreditMenu()
    {
        ClipCreditMenu.Play();
    }

    void PlayBackToOptionMenu()
    {
        ClipBackToOptionMenu.Play();
    }


    private void LaunchCanvasMainMenu()
    {
        MainMenu.gameObject.SetActive(true);
    }

    private void LaunchCanvasOptionsMenu()
    {
        OptionsMenu.gameObject.SetActive(true);
    }

    private void LaunchCanvasControlMenu()
    {
        ControlMenu.gameObject.SetActive(true);
    }

    private void LaunchCanvasCreditMenu()
    {
        CreditMenu.gameObject.SetActive(true);
    }

    public VideoPlayer ClipStart
    {
        get
        {
            return clipStart;
        }

        set
        {
            clipStart = value;
        }
    }

    public VideoPlayer ClipOption
    {
        get
        {
            return clipOption;
        }

        set
        {
            clipOption = value;
        }
    }

    public VideoPlayer ClipBackToMenu
    {
        get
        {
            return clipBackToMenu;
        }

        set
        {
            clipBackToMenu = value;
        }
    }

    public VideoPlayer ClipControlMenu
    {
        get
        {
            return clipControlMenu;
        }

        set
        {
            clipControlMenu = value;
        }
    }

    public VideoPlayer ClipBackToOptionMenu
    {
        get
        {
            return clipBackToOptionMenu;
        }

        set
        {
            clipBackToOptionMenu = value;
        }
    }

    public VideoPlayer ClipCreditMenu
    {
        get
        {
            return clipCreditMenu;
        }

        set
        {
            clipCreditMenu = value;
        }
    }

    public VideoPlayer ClipBackFromCreditToOptionMenu
    {
        get
        {
            return clipBackFromCreditToOptionMenu;
        }

        set
        {
            clipBackFromCreditToOptionMenu = value;
        }
    }

}
