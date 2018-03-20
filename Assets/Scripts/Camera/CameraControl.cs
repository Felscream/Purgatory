﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [SerializeField] private int moveSpeed = 10;
    [SerializeField] private float snapDistance = 0.05f;
    [SerializeField] private Transform mainAxis;
    [SerializeField] private Transform shakeAxis;
    [SerializeField] private float intensityReduction = 10.0f;
    [SerializeField] private float defaultOrthographicSize = 11.25f;
    [SerializeField] private float zoomOrthographicSize = 16.25f;
    [SerializeField] private float zoomDuration = 0.75f;
    private float baseX, baseY;
    private int shakeReduction = 10;
    private bool isShaking = false, isMoving = false;
    private int shakeCount;
    private float shakeIntensity;
    private float shakeSpeed;
    private float currentSpeed;
    private Vector3 nextShakePosition;
    private Vector3 newPosition;
    private ManagerInGame manager;
    private Camera mainCamera;
	// Use this for initialization
    void Awake()
    {
        //enabled = false;
        mainCamera = Camera.main;
        shakeAxis = mainCamera.GetComponent<Transform>();
        baseX = shakeAxis.localPosition.x;
        baseY = shakeAxis.localPosition.y;
    }
	void Start () {
        manager = ManagerInGame.GetInstance();
    }
	
	// Update is called once per frame
	void Update () {
        if (isMoving)
        {
            mainAxis.position = Vector3.MoveTowards(mainAxis.position, newPosition, Time.deltaTime * currentSpeed);

            if(Vector2.Distance(mainAxis.position, newPosition) < snapDistance)
            {
                mainAxis.position = newPosition;
                isMoving = false;
                if (!isShaking)
                {
                    enabled = true;
                }
            }
        }

        if (isShaking)
        {
            shakeAxis.localPosition = Vector3.MoveTowards(shakeAxis.localPosition, nextShakePosition, Time.deltaTime * shakeSpeed);

            if(Vector2.Distance(shakeAxis.localPosition, nextShakePosition) < shakeIntensity/5f)
            {
                shakeCount--;

                if(shakeCount <= 0)
                {
                    isShaking = false;
                    shakeAxis.localPosition = new Vector3(baseX, baseY, shakeAxis.localPosition.z);
                    if (!isMoving)
                    {
                        enabled = false;
                    }
                }
                else if(shakeCount <= 1)
                {
                    nextShakePosition = new Vector3(baseX, baseY, shakeAxis.localPosition.z);
                }
                else
                {
                    NextShakePosition();
                }
            }
        }
	}

    public void Shake(float intensity, int shakes, float speed, bool overridePlayerCount = false)
    {
        enabled = true;
        isShaking = true;
        shakeCount = shakes;
        if (!overridePlayerCount && manager != null)
        {
            int playerCount = manager.PlayerAlive != 0 ? manager.PlayerAlive : 1;
            shakeIntensity = intensity / (intensityReduction * playerCount);
        }
        else
        {
            shakeIntensity = intensity / intensityReduction;
        }
        shakeSpeed = speed;
    }

    private void NextShakePosition()
    {
        nextShakePosition = new Vector3(Random.Range(-shakeIntensity, shakeIntensity), 
            Random.Range(-shakeIntensity, shakeIntensity), shakeAxis.localPosition.z);

    }

    public void Move(float x, float y, float speed = 0)
    {
        if(speed > 0)
        {
            currentSpeed = speed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
        newPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
        isMoving = true;
        enabled = true;
    }

    public void SetPosition(Vector2 position)
    {
        Vector3 pos = new Vector3(position.x, position.y, mainAxis.position.z);
        mainAxis.position = pos;
    }

    public IEnumerator ZoomIn(Vector2 position, float waitTime)
    {
        Vector3 startingPosition = mainAxis.position;
        Vector3 endPosition = new Vector3(position.x, position.y, -1);
        StartCoroutine(ZoomOrthographic(mainCamera.orthographicSize, zoomOrthographicSize));
        float i = 0.0f;
        float rate = 1.0f / zoomDuration;
        while (i < 1.0)
        {
            i += Time.unscaledDeltaTime * rate;
            mainAxis.position = Vector3.Lerp(startingPosition, endPosition, i);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(zoomDuration + waitTime);
        StartCoroutine(ZoomOrthographic(mainCamera.orthographicSize, defaultOrthographicSize));
        i = 0.0f;
        while (i < 1.0)
        {
            i += Time.unscaledDeltaTime * rate;
            mainAxis.position = Vector3.Lerp(endPosition, startingPosition, i);
            yield return null;
        }
    }

    public IEnumerator ZoomOrthographic(float start, float end)
    {
        float i = 0.0f;
        float rate = 1.0f / zoomDuration;
        while (i < 1.0)
        {
            i += Time.unscaledDeltaTime * rate;
            mainCamera.orthographicSize = Mathf.Lerp(start, end, i);
            yield return null;
        }
    }
    public float ZoomDuration
    {
        get
        {
            return zoomDuration;
        }
    }
    public Transform MainAxis
    {
        get
        {
            return mainAxis;
        }
    }
}
