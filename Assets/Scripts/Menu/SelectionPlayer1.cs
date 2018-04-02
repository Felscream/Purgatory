using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionPlayer1 : MonoBehaviour
{
    // Animator for selected animation
    private Animator KnightPlayerAnim;
    private Animator ArcherPlayerAnim;
    private Animator SorcererPlayerAnim;
    
    // All sprite animated
    public RectTransform KnightPlayer;
    public RectTransform ArcherPlayer;
    public RectTransform SorcererPlayer;

    // all buttons
    [SerializeField] GameObject playerKnightButton;
    [SerializeField] GameObject playerArcherButton;
    [SerializeField] GameObject playerSorcererButton;

    // Sprite to show when player do something
    public RectTransform playerJoin;
    public RectTransform playerSelection;
    public RectTransform playerValidate;

    // Numero du joueur
    public int playerNumber;

    // Commande pour contrôler le menu
    public string Jump, StartJoystick, Horizontal, Dodge;

    // Default index of the buttonplayer1/2/3/4
    private int selectionIndexPlayer = 1;

    // Player choice
    private bool playerIsHere = false;
    private bool playerChoosed = false;

    // StartText must be displayed only if there are 2 or more players
    private Transform startText;
    
    // ChampionsSelected instance
    ChampionsSelected championsSelected_;

    // Joystick is running ?
    bool m_isAxisOneInUse = false;
    private X360_controller controller;

    // Use this for initialization
    void Start()
    {
        playerJoin.gameObject.SetActive(true);
        playerSelection.gameObject.SetActive(false);
        playerValidate.gameObject.SetActive(false);

        KnightPlayerAnim = KnightPlayer.GetComponent<Animator>();
        ArcherPlayerAnim = ArcherPlayer.GetComponent<Animator>();
        SorcererPlayerAnim = SorcererPlayer.GetComponent<Animator>();

        championsSelected_ = ChampionsSelected.GetInstance();

        startText = transform.parent.Find("StartText");
        controller = ControllerManager.Instance.GetController(playerNumber);
    }

    // Update is called once per frame
    void Update()
    {
        AddPlayers();

        SelectionAndDeselection();

        if (playerIsHere)
        {
            SelectionCharacterwithController();
        }

    }
    
    public void AddPlayers()
    {
        // Player 1 join the game
        if (controller.GetButtonDown("Start"))
        {
            if (!playerIsHere)
            {
                playerJoin.gameObject.SetActive(false);
                playerSelection.gameObject.SetActive(true);
                SelectedIndexPlayer();
                playerIsHere = true;
            }
            else
            {
                if(championsSelected_.PlayerNumber >= 2)
                {
                    SceneManager.LoadScene(2);
                }
            }

        }
    }

    public void SelectionAndDeselection()
    {
        // Player 1 validate his choice
        if(controller != null)
        {
            if (controller.GetButtonDown("A") && !playerChoosed)
            {
                playerChoosed = true;
                championsSelected_.playerSelection[playerNumber - 1] = selectionIndexPlayer;
                championsSelected_.PlayerNumber++;
                playerSelection.gameObject.SetActive(false);
                playerValidate.gameObject.SetActive(true);
                if (championsSelected_.PlayerNumber >= 2)
                {
                    startText.gameObject.SetActive(true);
                }
            }
            // Player 1 want to change his champion
            if (controller.GetButtonDown("B"))
            {
                if (playerChoosed)
                {
                    playerChoosed = false;
                    championsSelected_.playerSelection[playerNumber - 1] = 0;
                    championsSelected_.PlayerNumber--;
                    playerSelection.gameObject.SetActive(true);
                    playerValidate.gameObject.SetActive(false);
                    if (championsSelected_.PlayerNumber < 2)
                    {
                        startText.gameObject.SetActive(false);
                    }
                }
                else // Player doesn't want to be part of the game anymore
                {
                    playerJoin.gameObject.SetActive(true);
                    playerSelection.gameObject.SetActive(false);
                    playerIsHere = false;
                    selectionIndexPlayer = 1;
                }
            }
        }
        
        
    }

    public void SelectionCharacterwithController()
    {
        if (!m_isAxisOneInUse)
        {
            if (controller.GetStick_L().X < 0f)
            {
                m_isAxisOneInUse = true;
                DecreaseIndex();
                SelectedIndexPlayer();
            }
            else
            {
                if (controller.GetStick_L().X > 0f)
                {
                    m_isAxisOneInUse = true;
                    IncreaseIndex();
                    SelectedIndexPlayer();
                }
            }
        }

        if (controller.GetStick_L().X == 0 && m_isAxisOneInUse == true)
        {
            m_isAxisOneInUse = false;
        }
    }
    
    public void SelectedIndexPlayer()
    {
        switch (selectionIndexPlayer)
        {
            case 1:
                playerKnightButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                playerSorcererButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                playerArcherButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                // Changer sprite au lieu de mettre highligth
                KnightPlayer.gameObject.SetActive(true);
                SorcererPlayer.gameObject.SetActive(false);
                ArcherPlayer.gameObject.SetActive(false);
                break;
            case 2:
                playerKnightButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                playerSorcererButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                playerArcherButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                KnightPlayer.gameObject.SetActive(false);
                SorcererPlayer.gameObject.SetActive(true);
                ArcherPlayer.gameObject.SetActive(false);
                break;
            case 3:
                playerKnightButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                playerSorcererButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                playerArcherButton.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                KnightPlayer.gameObject.SetActive(false);
                SorcererPlayer.gameObject.SetActive(false);
                ArcherPlayer.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    public void DecreaseIndex()
    {
        if (selectionIndexPlayer > 1) selectionIndexPlayer--;
    }

    public void IncreaseIndex()
    {
        if (selectionIndexPlayer < 3) selectionIndexPlayer++;
    }
}
