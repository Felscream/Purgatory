﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ClipController : MonoBehaviour {

    VideoPlayer clipStart;
    VideoPlayer clipOption;
    VideoPlayer clipBackToMenu;

    public GameObject LaunchMainMenu;
    public GameObject LaunchOptionsClip;
    public GameObject LaunchBackToMenu;

    public RectTransform MainMenu;
    public RectTransform OptionsMenu;

    private void Awake()
    {
        MainMenu.gameObject.SetActive(false);
        OptionsMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(true);
        LaunchOptionsClip.SetActive(false);

        clipStart = LaunchMainMenu.GetComponent<VideoPlayer>();
        clipOption = LaunchOptionsClip.GetComponent<VideoPlayer>();
        clipBackToMenu = LaunchBackToMenu.GetComponent<VideoPlayer>();
    }

    // Use this for initialization
    void Start()
    {
        //MainMenu.gameObject.SetActive(false);
        PlayMainMenuFirstTime();
    }

    // Update is called once per frame
    void Update () {


        if (!clipStart.isPlaying && clipStart.time >= 1.0f)
        {
            clipStart.targetCameraAlpha = 0.6f;
            LaunchCanvasMainMenu();
        }

        if (!clipOption.isPlaying && clipOption.time >= 1.0f)
        {
            clipOption.targetCameraAlpha = 0.6f;
            LaunchCanvasOptionsMenu();
        }

        if (!clipBackToMenu.isPlaying && clipBackToMenu.time >= 1.0f)
        {
            clipBackToMenu.targetCameraAlpha = 0.6f;
            LaunchCanvasMainMenu();
        }
    }

    public void ChangeClipMainMenuToOptionsMenu()
    {
        MainMenu.gameObject.SetActive(false);

        LaunchMainMenu.gameObject.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(false);
        LaunchOptionsClip.SetActive(true);

        PlayOptionsMenu();
    }

    public void ChangeClipBackToMenu()
    {
        OptionsMenu.gameObject.SetActive(false);

        LaunchOptionsClip.SetActive(false);
        LaunchBackToMenu.gameObject.SetActive(true);

        PlayBackToMenu();
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

    private void LaunchCanvasMainMenu()
    {
        MainMenu.gameObject.SetActive(true);
    }

    private void LaunchCanvasOptionsMenu()
    {
        OptionsMenu.gameObject.SetActive(true);
    }

}