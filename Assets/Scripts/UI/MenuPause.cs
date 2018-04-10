using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour {


    public Button reprendre, options, quitter, retourControls;
    public ControllerManager controlManager;
    public ManagerInGame managerInGame;

    public GameObject controlsMenu, HUDCanvas;

    private X360_controller controller;
    private AudioVolumeManager audioManager;
    private bool isChanging = false;
    private bool onDisplay = false;
    private bool controlsOnDisplay = false;
    private Transform pauseMenu;

    private int selectedItem = 1;

    // Use this for initialization
    void Start() {
        pauseMenu = transform.GetChild(0);
        pauseMenu.gameObject.SetActive(false);
        audioManager = AudioVolumeManager.GetInstance();
    }

    // Update is called once per frame
    void Update() {

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

        if (onDisplay && !controlsOnDisplay)
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

    public void OnPause()
    {
        controller = controlManager.GetController(controlManager.getControllerIndexOnButtonDown("Start"));
        audioManager.PlaySoundEffect("Select");
        onDisplay = true;
        pauseMenu.gameObject.SetActive(true);

        Time.timeScale = 0.0f;
        Cursor.visible = true;
        managerInGame.PauseAgentsAudio();
        managerInGame.PauseNarratorAudio();
        //HUDCanvas.SetActive(false);
        selectedItem = 1;
        options.Select();
        reprendre.Select();
    }

    public void OnUnpause()
    {
        onDisplay = false;
        pauseMenu.gameObject.SetActive(false);

        //HUDCanvas.SetActive(true);
        Time.timeScale = managerInGame.CurrentTimeScale;
        managerInGame.UnpauseAgentsAudio();
        managerInGame.UnpauseNarratorAudio();
        audioManager.PlaySoundEffect("Cancel");
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

    public void ShowControls()
    {
        audioManager.PlaySoundEffect("Select");
        controlsMenu.SetActive(true);
        controlsOnDisplay = true;
        retourControls.Select();
    }
    public void HideControls()
    {
        audioManager.PlaySoundEffect("Cancel");
        controlsMenu.SetActive(false);
        controlsOnDisplay = false;
        reprendre.Select();
        selectedItem = 1;
    }


    private void Validate()
    {
        if (controlsOnDisplay)
        {
            HideControls();
            audioManager.PlaySoundEffect("Cancel");
            return;
        }
        switch (selectedItem)
        {
            case 1:
                OnUnpause();
                audioManager.PlaySoundEffect("Cancel");
                break;
            case 2:
                ShowControls();
                audioManager.PlaySoundEffect("Select");
                break;
            default:
                managerInGame.LoadMainMenu();
                audioManager.PlaySoundEffect("Select");
                break;
        }
    }
    private void Cancel()
    {
        if (!controlsOnDisplay)
        {
            OnUnpause();
        } else
        {
            HideControls();
        }
    }
}
