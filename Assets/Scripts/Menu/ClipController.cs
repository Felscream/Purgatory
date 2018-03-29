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

    public GameObject LaunchMainMenu;
    public GameObject LaunchOptionsClip;
    public GameObject LaunchBackToMenu;
    public GameObject LaunchControlMenu;
    public GameObject LaunchBackToOptionMenu;

    public RectTransform MainMenu;
    public RectTransform OptionsMenu;
    public RectTransform ControlMenu;

    private AudioVolumeManager audioVolumeManager;
    private void Awake()
    {
        
        MainMenu.gameObject.SetActive(false);
        OptionsMenu.gameObject.SetActive(false);
        ControlMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(true);
        //LaunchOptionsClip.SetActive(false);

        clipStart = LaunchMainMenu.GetComponent<VideoPlayer>();
        clipOption = LaunchOptionsClip.GetComponent<VideoPlayer>();
        clipBackToMenu = LaunchBackToMenu.GetComponent<VideoPlayer>();
        clipControlMenu = LaunchControlMenu.GetComponent<VideoPlayer>();
        clipBackToOptionMenu = LaunchBackToOptionMenu.GetComponent<VideoPlayer>();
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


        if (!clipStart.isPlaying && clipStart.time >= 1.0f)
        {
            clipStart.targetCameraAlpha = 0.4f;
            LaunchCanvasMainMenu();
        }

        if (!clipOption.isPlaying && clipOption.time >= 1.0f)
        {
            clipOption.targetCameraAlpha = 0.4f;
            clipBackToMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasOptionsMenu();
        }

        if (!clipBackToMenu.isPlaying && clipBackToMenu.time >= 1.0f)
        {
            clipOption.targetCameraAlpha = 1.0f;
            clipBackToMenu.targetCameraAlpha = 0.4f;
            LaunchCanvasMainMenu();
        }

        if (!clipControlMenu.isPlaying && clipControlMenu.time >= 0.5f)
        {
            clipControlMenu.targetCameraAlpha = 0.0f;
            clipBackToOptionMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasControlMenu();
        }

        if (!clipBackToOptionMenu.isPlaying && clipBackToOptionMenu.time >= 0.5f)
        {
            clipControlMenu.targetCameraAlpha = 1.0f;
            clipBackToOptionMenu.targetCameraAlpha = 0.4f;
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

        LaunchControlMenu.SetActive(true);

        PlayControlMenu();
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
        LaunchBackToMenu.gameObject.SetActive(true);

        LaunchBackToOptionMenu.SetActive(true);

        PlayBackToOptionMenu();
    }

    void PlayBackToMenu()
    {
        clipBackToMenu.Play();
    }

    void PlayOptionsMenu()
    {
        clipOption.Play();
    }

    private void PlayMainMenuFirstTime()
    {
        clipStart.Play();
    }

    private void PlayControlMenu()
    {
        clipControlMenu.Play();
    }

    void PlayBackToOptionMenu()
    {
        clipBackToOptionMenu.Play();
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

}
