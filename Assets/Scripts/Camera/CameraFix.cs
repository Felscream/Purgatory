﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFix : MonoBehaviour
{

    [Range(1, 4)]
    public int pixelScale = 2;

    private int pixelsPerUnit = 16;
    private float halfScreen = 0.5f;
    private bool isFullscreen = true;
    private Camera _camera;

    private void Start()
    {
        //Screen.SetResolution(1920, 1080, isFullscreen, 30);
    }

    void Update()
    {
        // getting the camera
        // note: this could be done in Start()
        // however when running in Editor Mode you might get a null Camera here, this is mostly for helping newbies
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;
        }
        _camera.orthographicSize = Screen.height * ((halfScreen / pixelsPerUnit) / pixelScale);
    }
}
