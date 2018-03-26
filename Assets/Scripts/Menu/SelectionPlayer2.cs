using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionPlayer2 : MonoBehaviour
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

    // Commande pour contrôler le menu
    public string Jump, StartJoystick, Horizontal, Dodge;

    // Default index of the buttonplayer1/2/3/4
    private int selectionIndexPlayer = 0;
    private int playerNumber = 0;

    // Player choice
    private bool playerIsHere = false;
    private bool playerChoosed = false;

    // ChampionsSelected instance
    ChampionsSelected championsSelected_;

    // Joystick is running ?
    bool m_isAxisOneInUse = false;

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
        if (Input.GetButtonDown(StartJoystick) && !playerIsHere)
        {
            playerJoin.gameObject.SetActive(false);
            playerSelection.gameObject.SetActive(true);
            SelectedIndexPlayer();
            playerIsHere = true;
        }
    }

    public void SelectionAndDeselection()
    {
        // Player 1 validate his choice
        if (Input.GetButtonDown(Jump) && !playerChoosed)
        {
            playerChoosed = true;
            championsSelected_.playerSelection.Insert(1, selectionIndexPlayer);
            championsSelected_.PlayerNumber++;
            playerSelection.gameObject.SetActive(false);
            playerValidate.gameObject.SetActive(true);
        }
        // Player 1 want to change his champion
        if (Input.GetButtonDown(Dodge) && playerChoosed)
        {
            playerChoosed = false;
            championsSelected_.playerSelection.Insert(1, 0);
            championsSelected_.PlayerNumber--;
            playerSelection.gameObject.SetActive(true);
            playerValidate.gameObject.SetActive(false);
        }

    }

    public void SelectionCharacterwithController()
    {
        if (!m_isAxisOneInUse)
        {
            if (Input.GetAxisRaw(Horizontal) == -1)
            {
                m_isAxisOneInUse = true;
                DecreaseIndex();
                SelectedIndexPlayer();
            }
            else
            {
                if (Input.GetAxisRaw(Horizontal) == 1)
                {
                    m_isAxisOneInUse = true;
                    IncreaseIndex();
                    SelectedIndexPlayer();
                }
            }
        }

        if (Input.GetAxisRaw(Horizontal) == 0 && m_isAxisOneInUse == true)
        {
            m_isAxisOneInUse = false;
        }
    }

    public void SelectedIndexPlayer()
    {
        switch (selectionIndexPlayer)
        {
            case 1:
                playerKnightButton.GetComponent<Button>().Select();
                KnightPlayer.gameObject.SetActive(true);
                SorcererPlayer.gameObject.SetActive(false);
                ArcherPlayer.gameObject.SetActive(false);
                break;
            case 2:
                playerSorcererButton.GetComponent<Button>().Select();
                KnightPlayer.gameObject.SetActive(false);
                SorcererPlayer.gameObject.SetActive(true);
                ArcherPlayer.gameObject.SetActive(false);
                break;
            case 3:
                playerArcherButton.GetComponent<Button>().Select();
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
        if (selectionIndexPlayer > 0) selectionIndexPlayer--;
    }

    public void IncreaseIndex()
    {
        if (selectionIndexPlayer < 3) selectionIndexPlayer++;
    }
}
