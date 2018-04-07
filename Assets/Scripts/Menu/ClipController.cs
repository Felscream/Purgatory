using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
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
    private Animator anim;

    private Button classement;
    private Button retour;
    private void Awake()
    {
        
        MainMenu.gameObject.SetActive(false);
        OptionsMenu.gameObject.SetActive(false);
        ControlMenu.gameObject.SetActive(false);
        CreditMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(true);

        clipStart = LaunchMainMenu.GetComponent<VideoPlayer>();
        clipOption = LaunchOptionsClip.GetComponent<VideoPlayer>();
        clipBackToMenu = LaunchBackToMenu.GetComponent<VideoPlayer>();
        clipControlMenu = LaunchControlMenu.GetComponent<VideoPlayer>();
        clipBackToOptionMenu = LaunchBackToOptionMenu.GetComponent<VideoPlayer>();
        clipCreditMenu = LaunchCreditMenu.GetComponent<VideoPlayer>();
        clipBackFromCreditToOptionMenu = LaunchBackFromCreditToOptionMenu.GetComponent<VideoPlayer>();
    }

    // Use this for initialization
    void Start()
    {
        anim = MainMenu.transform.parent.GetComponent<Animator>();
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
            
            clipBackToMenu.targetCameraAlpha = 0.4f;
            LaunchCanvasMainMenu();
            clipOption.targetCameraAlpha = 1.0f;
        }

        if (!clipControlMenu.isPlaying && clipControlMenu.time >= 0.5f)
        {
            clipControlMenu.targetCameraAlpha = 0.0f;
            clipBackToOptionMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasControlMenu();
        }

        if (!clipCreditMenu.isPlaying && clipCreditMenu.time >= 0.5f)
        {
            clipCreditMenu.targetCameraAlpha = 0.0f;
            clipBackToOptionMenu.targetCameraAlpha = 1.0f;
            LaunchCanvasCreditMenu();
        }

        if (!clipBackToOptionMenu.isPlaying && clipBackToOptionMenu.time >= 0.5f)
        {
            clipControlMenu.targetCameraAlpha = 1.0f;
            clipBackToOptionMenu.targetCameraAlpha = 0.4f;
            LaunchCanvasOptionsMenu();
        }

        if (!clipBackFromCreditToOptionMenu.isPlaying && clipBackFromCreditToOptionMenu.time >= 0.5f)
        {
            clipCreditMenu.targetCameraAlpha = 1.0f;
            clipBackFromCreditToOptionMenu.targetCameraAlpha = 0.4f;
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
    public void ChangeClipMainMenuToLeaderboardMenu()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        MainMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchControlMenu.gameObject.SetActive(false);
        LaunchBackToOptionMenu.gameObject.SetActive(false);

        //LaunchOptionsClip.SetActive(true);

        
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
        clipBackFromCreditToOptionMenu.Play();
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

    private void PlayCreditMenu()
    {
        clipCreditMenu.Play();
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

    private void LaunchCanvasCreditMenu()
    {
        CreditMenu.gameObject.SetActive(true);
    }

    public void LaunchCanvasLeaderboard()
    {
        anim.SetTrigger("sRight");
        audioVolumeManager.PlaySoundEffect("Select");
        anim.ResetTrigger("sLeft");
        classement = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        classement.interactable = false;
        if(retour != null)
        {
            retour.interactable = true;
        }
        
    }

    public void LaunchCanvasMenuFromLeaderboard()
    {
        anim.SetTrigger("sLeft");
        audioVolumeManager.PlaySoundEffect("Cancel");
        anim.ResetTrigger("sRight");
        retour = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        retour.interactable = false;
        classement.interactable = true;
    }
}
