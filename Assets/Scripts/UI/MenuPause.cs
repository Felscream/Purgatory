using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour {


    public Button reprendre, options, quitter;
    public ControllerManager controlManager;

    private X360_controller controller;

    private bool isChanging = false;
    private bool onDisplay = false;
    private Transform pauseMenu;

    private int selectedItem = 1;

	// Use this for initialization
	void Start () {
        pauseMenu = transform.GetChild(0);
        pauseMenu.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

		if (controlManager.GetButtonDownAny("Start"))
        {
            if (!onDisplay)
            {
                OnPause();
            }
            else
            {
                OnUnpause();
            }
        }

        if (onDisplay)
        {
            if (controller != null && controller.GetStick_L().Y < -.2 && !isChanging)
            {
                switchItem(false);
            }
            if (controller != null && controller.GetStick_L().Y > .2 && !isChanging)
            {
                switchItem(true);
            }
            if (controller != null && Mathf.Abs(controller.GetStick_L().Y) < .2)
            {
                isChanging = false;
            }


        }
	}

    private void OnPause()
    {
        controller = controlManager.GetController(controlManager.getControllerIndexOnButtonDown("Start"));

        onDisplay = true;
        pauseMenu.gameObject.SetActive(true);

        Time.timeScale = 0.00001f;
        Cursor.visible = true;

        reprendre.Select();
        selectedItem = 1;
    }

    private void OnUnpause()
    {
        onDisplay = false;
        pauseMenu.gameObject.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    private void switchItem(bool upClicked)
    {
        isChanging = true;
        if (upClicked)
        {
            selectedItem--;
        } else
        {
            selectedItem++;
        }
        if (selectedItem == 0)
            selectedItem = 3;
        if (selectedItem == 4)
            selectedItem = 1;
        
        switch (selectedItem)
        {
            case 1:
                reprendre.Select();
                break;
            case 2:
                options.Select();
                break;
            default:
                quitter.Select();
                break;
        }
    }
}
