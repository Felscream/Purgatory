using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour {

    AudioVolumeManager audioVolumeManager;

    private int panelIndex;
    private int mainMenuIndex = 1;
    private int optionMenuIndex = 1;

    private X360_controller controller;

    // Joystick is running ?
    bool m_isAxisOneInUse = false;
    bool changePanel = false;

    // all buttons
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject optionButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject optionControlButton;
    [SerializeField] GameObject optionBackButton;
    [SerializeField] GameObject optionCreditsButton;
    [SerializeField] GameObject controlBackButton;
    [SerializeField] GameObject creditsBackButton;

    public GameObject clipMenu;
    ClipController clipController_;


    // Use this for initialization
    void Start () {
        clipController_ = clipMenu.GetComponent<ClipController>();

        panelIndex = 1;
        controller = ControllerManager.Instance.GetController(1);
        SelectedIndexMenu();

        audioVolumeManager = AudioVolumeManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // Player 1 validate his choice
        if (controller != null && changePanel)
        {
            SelectedIndexMenu();
        }
        SelectionWithController();
        PressedButton();
    }

    private void PressedButton()
    {
        if (controller.GetButtonDown("A"))
        {
            switch (panelIndex)
            {
                case 1: // Menu Principal
                    switch (mainMenuIndex)
                    {
                        case 1: // Jouer
                            LoadLobbyScene();
                            break;
                        case 2: // Options
                            clipController_.ChangeClipMainMenuToOptionsMenu();
                            break;
                        case 3: // Quitter
                            QuitGame();
                            break;
                        default:
                            print("Incorrect intelligence level.");
                            break;
                    }
                    break;
                case 2: // Options
                    break;
                case 3: // Contrôles
                    break;
                case 4: // Crédits
                    break;
                default:
                    print("Incorrect intelligence level.");
                    break;
            }
        }
    }



    public void LoadLobbyScene()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void QuitGame()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        Debug.Log("Quit!");
        Application.Quit();
    }


    public void SelectedIndexMenu()
    {
        switch (panelIndex)
        {
            case 1: // Menu Principal
                playButton.GetComponent<Button>().Select();
                break;
            case 2: // Options
                optionControlButton.GetComponent<Button>().Select();
                break;
            case 3: // Contrôles
                controlBackButton.GetComponent<Button>().Select();
                break;
            case 4: // Crédits
                creditsBackButton.GetComponent<Button>().Select();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
        // Selection du boutton en fonction du panel activé
        SelectionWithController();
        changePanel = false;
    }

    public void ChangeIndex(int i)
    {
        panelIndex = i;
        changePanel = true;
    }

    public void SelectionWithController()
    {
        switch (panelIndex)
        {
            case 1: // Menu Principal
                if (!m_isAxisOneInUse)
                {
                    //Debug.Log(ControllerManager.Instance.GetController(1).IsConnected);
                    if (controller.GetStick_L().Y < 0f)
                    {
                        m_isAxisOneInUse = true;
                        IncreaseMainMenuIndex();
                    }
                    else
                    {
                        if (controller.GetStick_L().Y > 0f)
                        {
                            m_isAxisOneInUse = true;
                            DecreaseMainMenuIndex();
                        }
                    }
                }

                if (controller.GetStick_L().Y == 0 && m_isAxisOneInUse == true)
                {
                    m_isAxisOneInUse = false;
                }
                break;
            case 2: // Options
                optionControlButton.GetComponent<Button>().Select();
                if (!m_isAxisOneInUse)
                {
                    //Debug.Log(ControllerManager.Instance.GetController(1).IsConnected);
                    if (controller.GetStick_L().Y < 0f)
                    {
                        m_isAxisOneInUse = true;
                        IncreaseOptionMenuIndex();
                    }
                    else
                    {
                        if (controller.GetStick_L().Y > 0f)
                        {
                            m_isAxisOneInUse = true;
                            DecreaseOptionMenuIndex();
                        }
                    }
                }

                if (controller.GetStick_L().Y == 0 && m_isAxisOneInUse == true)
                {
                    m_isAxisOneInUse = false;
                }
                break;
            case 3: // Contrôles
                controlBackButton.GetComponent<Button>().Select();
                break;
            case 4: // Crédits
                creditsBackButton.GetComponent<Button>().Select();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }

        
    }
    
    private void DecreaseMainMenuIndex()
    {
        if (mainMenuIndex > 0) mainMenuIndex--;
        if (mainMenuIndex == 0) mainMenuIndex = 3;
        switchMainMenu();
    }

    private void IncreaseMainMenuIndex()
    {
        if (mainMenuIndex < 4) mainMenuIndex++;
        if (mainMenuIndex == 4) mainMenuIndex = 1;
        switchMainMenu();
    }

    private void switchMainMenu()
    {
        switch (mainMenuIndex)
        {
            case 1:
                playButton.GetComponent<Button>().Select();
                /*
                playButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                optionButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                quitButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                */
                break;
            case 2:
                optionButton.GetComponent<Button>().Select();
                /*
                optionButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                playButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                quitButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                */
                break;
            case 3:
                quitButton.GetComponent<Button>().Select();
                /*
                quitButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                playButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                optionButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                */
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    private void DecreaseOptionMenuIndex()
    {
        if (optionMenuIndex > 1) optionMenuIndex--;
        if (optionMenuIndex == 1) optionMenuIndex = 3;

        switch (optionMenuIndex)
        {
            case 1:
                optionControlButton.GetComponent<Button>().Select();
                break;
            case 2:
                optionCreditsButton.GetComponent<Button>().Select();
                break;
            case 3:
                optionBackButton.GetComponent<Button>().Select();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    private void IncreaseOptionMenuIndex()
    {
        if (optionMenuIndex < 3) optionMenuIndex++;
        if (optionMenuIndex == 3) optionMenuIndex = 1;

        switch (optionMenuIndex)
        {
            case 1:
                playButton.GetComponent<Button>().Select();
                break;
            case 2:
                optionButton.GetComponent<Button>().Select();
                break;
            case 3:
                quitButton.GetComponent<Button>().Select();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

}
