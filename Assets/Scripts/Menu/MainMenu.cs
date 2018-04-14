using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    AudioVolumeManager audioVolumeManager;

    // Le panel dans lequel on se trovue
    private int panelIndex;
    // Le bouton actuellement selectionné dans le mainpanel 
    private int mainMenuIndex;
    // Le bouton actuellement selectionné dans le optionpanel 
    private int optionMenuIndex;
    private SceneController sceneController;
    private Animator anim;
    private X360_controller controller;
    [SerializeField] private Color sliderActiveColor;
    [SerializeField] private Color sliderInactiveColor;

    // Joystick is running ?
    bool m_isAxisOneInUse = false;
    bool changePanel = false;
    bool musicSliderActivated = false;
    bool effectSliderActivated = false;
    bool voiceSliderActivated = false;

    // all buttons
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject leaderboardButton;
    [SerializeField] GameObject optionButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject optionControlButton;
    [SerializeField] GameObject optionCreditsButton;

    [SerializeField] GameObject optionMusicButton;
    [SerializeField] Slider optionMusicSlider;
    [SerializeField] GameObject optionEffectButton;
    [SerializeField] Slider optionEffectSlider;
    [SerializeField] GameObject optionVoiceButton;
    [SerializeField] Slider optionVoiceSlider;

    [SerializeField] GameObject optionBackButton;
    [SerializeField] GameObject leaderboardBackButton;

    public GameObject clipMenu;
    ClipController clipController_;


    // Use this for initialization
    void Start () {
        clipController_ = clipMenu.GetComponent<ClipController>();
        mainMenuIndex = 1;
        panelIndex = 1;
        optionMenuIndex = 1;
        controller = ControllerManager.Instance.GetController(1);
        anim = GetComponent<Animator>();
        SelectedIndexMenu();
        switchMainMenu();
        audioVolumeManager = AudioVolumeManager.GetInstance();
        sceneController = SceneController.GetInstance();
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.GetButtonDown("A") || controller.GetButtonDown("B"))
        {
            Debug.Log(panelIndex);
        }
        // Player 1 validate his choice
        if (controller != null && changePanel)
        {
            SelectedIndexMenu();
        }
        
        if (!musicSliderActivated && !effectSliderActivated && !voiceSliderActivated && !IsAnyClipPlaying())
        {
            SelectionWithController();
            PressedButton();
        }

        if (musicSliderActivated)
        {
            ControlSliderMusicEffectVoice(1);
        }

        if (effectSliderActivated)
        {
            ControlSliderMusicEffectVoice(2);
        }

        if (voiceSliderActivated)
        {
            ControlSliderMusicEffectVoice(3);
        }
    }

    private bool IsAnyClipPlaying()
    {
        bool isAnyClipPlaying;
        if (!clipController_.ClipStart.isPlaying && clipController_.ClipStart.time >= 1.0f || !clipController_.ClipBackToMenu.isPlaying && clipController_.ClipBackToMenu.time >= 1.0f || !clipController_.ClipOption.isPlaying && clipController_.ClipOption.time >= 1.0f || !clipController_.ClipCreditMenu.isPlaying && clipController_.ClipCreditMenu.time >= 0.5f || (!clipController_.ClipControlMenu.isPlaying && clipController_.ClipControlMenu.time >= 0.5f) || !clipController_.ClipBackFromCreditToOptionMenu.isPlaying && clipController_.ClipBackFromCreditToOptionMenu.time >= 0.5f || !clipController_.ClipBackToOptionMenu.isPlaying && clipController_.ClipBackToOptionMenu.time >= 0.5f)
        {
            isAnyClipPlaying = true;
        }
        isAnyClipPlaying = false;
        return isAnyClipPlaying;
    }

    private void PressedButton()
    {
        // Appuie sur A
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
                        case 2: //Leaderboard
                            LaunchLeaderboardCanvas();
                            ChangeIndex(2);
                            break;
                        case 3: // Options
                            clipController_.ChangeClipMainMenuToOptionsMenu();
                            ChangeIndex(3);
                            break;
                        case 4: // Quitter
                            QuitGame();
                            break;
                        default:
                            print("Incorrect intelligence level.");
                            break;
                    }
                    break;
                case 2:
                    LaunchMenuFromLeaderboard();
                    ChangeIndex(1);
                    break;
                case 3: // Options
                    switch (optionMenuIndex)
                    {
                        case 1: // Contrôles
                            clipController_.ChangeClipOptionsMenuToControlMenu();
                            ChangeIndex(4);
                            break;
                        case 2: // Crédits
                            clipController_.ChangeClipOptionsMenuToCreditMenu();
                            ChangeIndex(5);
                            break;
                        case 3: // Musique
                            musicSliderActivated = true;
                            break;
                        case 4: // Effets sonores
                            effectSliderActivated = true;
                            break;
                        case 5: // Voix
                            voiceSliderActivated = true;
                            break;
                        case 6: // Retour
                            clipController_.ChangeClipBackToMenu();
                            ChangeIndex(1);
                            break;
                        default:
                            print("Incorrect intelligence level.");
                            break;
                    }
                    break;
                case 4: // Contrôles
                    break;
                case 5: // Crédits
                    break;
                default:
                    print("Incorrect intelligence level.");
                    break;
            }
        }
        // Appuie sur B
        if (controller.GetButtonDown("B"))
        {
            switch (panelIndex)
            {
                case 2:
                    LaunchMenuFromLeaderboard();
                    ChangeIndex(1);
                    break;
                case 3: //options
                    // Retour
                    clipController_.ChangeClipBackToMenu();
                    ChangeIndex(1);
                    break;
                case 4: // Contrôles
                    // Retour
                    clipController_.ChangeClipControlBackToOptionMenu();
                    ChangeIndex(3);
                    break;
                case 5:// Crédits
                    clipController_.ChangeClipControlBackFromCreditToOptionMenu();
                    ChangeIndex(3);
                    break;
                default:
                    print("Incorrect intelligence level.");
                    break;
            }
        }
    }

    private void ControlSliderMusicEffectVoice(int indexSlider)
    {
        switch (indexSlider)
        {
            case 1:
                optionMusicSlider.GetComponentInChildren<Image>().color = sliderActiveColor;
                if (controller.GetStick_L().X < 0f)
                {
                    if (optionMusicSlider.value > 0.0f)
                        optionMusicSlider.value = optionMusicSlider.value - 0.005f;
                }
                if (controller.GetStick_L().X > 0f)
                {
                    if (optionMusicSlider.value < 1.0f)
                        optionMusicSlider.value = optionMusicSlider.value + 0.005f;
                }

                // Appuie sur B
                if (controller.GetButtonDown("B"))
                {
                    musicSliderActivated = false;
                    optionMusicSlider.GetComponentInChildren<Image>().color = sliderInactiveColor;
                }
                break;
            case 2:
                optionEffectSlider.GetComponentInChildren<Image>().color = sliderActiveColor;
                if (controller.GetStick_L().X < 0f)
                {
                    if (optionEffectSlider.value > 0.0f)
                        optionEffectSlider.value = optionEffectSlider.value - 0.005f;
                }
                if (controller.GetStick_L().X > 0f)
                {
                    if (optionEffectSlider.value < 1.0f)
                        optionEffectSlider.value = optionEffectSlider.value + 0.005f;
                }

                // Appuie sur B
                if (controller.GetButtonDown("B"))
                {
                    effectSliderActivated = false;
                    optionEffectSlider.GetComponentInChildren<Image>().color = sliderInactiveColor;
                }
                break;

            case 3:
                optionVoiceSlider.GetComponentInChildren<Image>().color = sliderActiveColor;
                if (controller.GetStick_L().X < 0f)
                {
                    if (optionVoiceSlider.value > 0.0f)
                        optionVoiceSlider.value = optionVoiceSlider.value - 0.005f;
                }
                if (controller.GetStick_L().X > 0f)
                {
                    if (optionVoiceSlider.value < 1.0f)
                        optionVoiceSlider.value = optionVoiceSlider.value + 0.005f;
                }

                // Appuie sur B
                if (controller.GetButtonDown("B"))
                {
                    voiceSliderActivated = false;
                    optionVoiceSlider.GetComponentInChildren<Image>().color = sliderInactiveColor;
                }
                break;
        }        
    }

    public void LoadLobbyScene()
    {
        audioVolumeManager.PlaySoundEffect("Select");
        sceneController.StartCoroutine(sceneController.LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
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
                switch (mainMenuIndex)
                {
                    case 1:
                        playButton.GetComponent<Button>().Select();
                        break;
                    case 2:
                        leaderboardButton.GetComponent<Button>().Select();
                        break;
                    case 3:
                        optionControlButton.GetComponent<Button>().Select();
                        break;
                    case 4:
                        quitButton.GetComponent<Button>().Select();
                        break;
                }
                
                break;
            case 2: // LeaderBoard
                leaderboardButton.GetComponent<Button>().Select();
                break;
            case 3: // Options
                Debug.Log("Je passe ici");
                optionControlButton.GetComponent<Button>().Select();
                break;
            case 4: // Contrôles
                // Juste le bouton B
                break;
            case 5: // Crédits
                // Juste le bouton B
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
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
            case 2: //leaderboard
                break;
            case 3: // Options
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
            case 4: // Contrôles
                break;
            case 5: // Crédits
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }
    
    private void DecreaseMainMenuIndex()
    {
        if (mainMenuIndex > 0) mainMenuIndex--;
        if (mainMenuIndex == 0) mainMenuIndex = 4;
        switchMainMenu();
    }

    private void IncreaseMainMenuIndex()
    {
        if (mainMenuIndex < 5) mainMenuIndex++;
        if (mainMenuIndex == 5) mainMenuIndex = 1;
        switchMainMenu();
    }

    private void switchMainMenu()
    {
        switch (mainMenuIndex)
        {
            case 1:
                playButton.GetComponent<Button>().Select();
                break;
            case 2:
                leaderboardButton.GetComponent<Button>().Select();
                break;
            case 3:
                optionButton.GetComponent<Button>().Select();
                break;
            case 4:
                quitButton.GetComponent<Button>().Select();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    private void DecreaseOptionMenuIndex()
    {
        if (optionMenuIndex > 0) optionMenuIndex--;
        if (optionMenuIndex == 0) optionMenuIndex = 6;
        switchOptionMenu();
    }

    private void IncreaseOptionMenuIndex()
    {
        if (optionMenuIndex < 7) optionMenuIndex++;
        if (optionMenuIndex == 7) optionMenuIndex = 1;
        switchOptionMenu();
    }


    private void switchOptionMenu()
    {
        switch (optionMenuIndex)
        {
            case 1:
                optionControlButton.GetComponent<Button>().Select();
                break;
            case 2:
                optionCreditsButton.GetComponent<Button>().Select();
                break;
            case 3:
                optionMusicButton.GetComponent<Button>().Select();
                break;
            case 4:
                optionEffectButton.GetComponent<Button>().Select();
                break;
            case 5:
                optionVoiceButton.GetComponent<Button>().Select();
                break;
            case 6:
                optionBackButton.GetComponent<Button>().Select();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    public void LaunchLeaderboardCanvas()
    {
        anim.SetTrigger("sRight");
        audioVolumeManager.PlaySoundEffect("Select");
        leaderboardBackButton.GetComponent<Button>().Select();
        leaderboardButton.GetComponent<Button>().interactable = false;
        leaderboardBackButton.GetComponent<Button>().interactable = true;
    }
    public void LaunchMenuFromLeaderboard()
    {
        anim.SetTrigger("sLeft");
        audioVolumeManager.PlaySoundEffect("Cancel");
        leaderboardButton.GetComponent<Button>().interactable = true;
        leaderboardBackButton.GetComponent<Button>().interactable = false;

    }
}
