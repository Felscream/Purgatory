using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionPlayer : MonoBehaviour
{
    // Animator for selected animation
    private Animator KnightPlayerAnim;
    private Animator ArcherPlayerAnim;
    private Animator SorcererPlayerAnim;
    
    // All sprite animated (need to be generalize)
    public RectTransform KnightPlayer1;
    public RectTransform ArcherPlayer1;
    public RectTransform SorcererPlayer1;
    public RectTransform KnightPlayer2;
    public RectTransform ArcherPlayer2;
    public RectTransform SorcererPlayer2;
    public RectTransform KnightPlayer3;
    public RectTransform ArcherPlayer3;
    public RectTransform SorcererPlayer3;
    public RectTransform KnightPlayer4;
    public RectTransform ArcherPlayer4;
    public RectTransform SorcererPlayer4;

    // all buttons
    [SerializeField] GameObject player1KnightButton;
    [SerializeField] GameObject player1ArcherButton;
    [SerializeField] GameObject player1SorcererButton;
    [SerializeField] GameObject player2KnightButton;
    [SerializeField] GameObject player2ArcherButton;
    [SerializeField] GameObject player2SorcererButton;
    [SerializeField] GameObject player3KnightButton;
    [SerializeField] GameObject player3ArcherButton;
    [SerializeField] GameObject player3SorcererButton;
    [SerializeField] GameObject player4KnightButton;
    [SerializeField] GameObject player4ArcherButton;
    [SerializeField] GameObject player4SorcererButton;

    // Sprite to show when player do something
    private RectTransform player1Join, player2Join, player3Join, player4Join;
    private RectTransform player1Selection, player2Selection, player3Selection, player4Selection;
    private RectTransform player1Validate, player2Validate, player3Validate, player4Validate;
    
    // Commande pour contrôler le menu
    public string Jump_P1, Jump_P2, Jump_P3, Jump_P4, Start_P1, Start_P2, Start_P3, Start_P4, Horizontal_P1, Horizontal_P2, Horizontal_P3, Horizontal_P4, Dodge_P1, Dodge_P2, Dodge_P3, Dodge_P4;

    // Default index of the buttonplayer1/2/3/4
    private int selectionIndexPlayer1 = 0, selectionIndexPlayer2 = 0, selectionIndexPlayer3 = 0, selectionIndexPlayer4 = 0;
    private int playerNumber = 0;

    // Player choice
    private bool player1IsHere = false, player2IsHere = false, player3IsHere = false, player4IsHere = false;
    private bool player1Choosed = false, player2Choosed = false, player3Choosed = false, player4Choosed = false;
    
    // ChampionsSelected instance
    ChampionsSelected championsSelected_;

    // Joystick is running ?
    bool m_isAxisOneInUse = false, m_isAxisTwoInUse = false, m_isAxisThreeInUse = false, m_isAxisFourInUse = false;

    // Use this for initialization
    void Start()
    {
        player1Join = (RectTransform)this.transform.GetChild(0);
        player1Join.gameObject.SetActive(true);
        player2Join = (RectTransform)this.transform.GetChild(1);
        player2Join.gameObject.SetActive(true);
        player3Join = (RectTransform)this.transform.GetChild(2);
        player3Join.gameObject.SetActive(true);
        player4Join = (RectTransform)this.transform.GetChild(3);
        player4Join.gameObject.SetActive(true);

        player1Selection = (RectTransform)this.transform.GetChild(4);
        player1Selection.gameObject.SetActive(false);
        player2Selection = (RectTransform)this.transform.GetChild(5);
        player2Selection.gameObject.SetActive(false);
        player3Selection = (RectTransform)this.transform.GetChild(6);
        player3Selection.gameObject.SetActive(false);
        player4Selection = (RectTransform)this.transform.GetChild(7);
        player4Selection.gameObject.SetActive(false);

        player1Validate = (RectTransform)this.transform.GetChild(8);
        player1Validate.gameObject.SetActive(false);

        KnightPlayerAnim = KnightPlayer1.GetComponent<Animator>();
        ArcherPlayerAnim = ArcherPlayer1.GetComponent<Animator>();
        SorcererPlayerAnim = SorcererPlayer1.GetComponent<Animator>();

        championsSelected_ = ChampionsSelected.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        AddPlayers();

        SelectionAndDeselection();

        if (player1IsHere)
        {
            SelectionCharacterwithController1();
        }

        if (player2IsHere)
        {
            SelectionCharacterwithController2();
        }

        if (player3IsHere)
        {
            SelectionCharacterwithController3();
        }

        if (player4IsHere)
        {
            SelectionCharacterwithController4();
        }

        LoadLevel();
    }

    public void LoadLevel()
    {
        if (Input.GetButtonDown(Start_P1) && (player1Choosed || (player1Choosed && player2Choosed) || (player1Choosed && player3Choosed) || (player1Choosed && player4Choosed) || (player1Choosed && player2Choosed && player3Choosed) || (player1Choosed && player3Choosed && player4Choosed) || (player1Choosed && player2Choosed && player4Choosed) || (player1Choosed && player2Choosed && player3Choosed && player4Choosed)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void AddPlayers()
    {
        // Player 1 join the game
        if (Input.GetButtonDown(Start_P1) && !player1IsHere)
        {
            player1Join.gameObject.SetActive(false);
            player1Selection.gameObject.SetActive(true);
            SelectedIndexPlayer1();
            player1IsHere = true;
        }
        // Player 1 join the game
        if (Input.GetButtonDown(Start_P2) && !player2IsHere)
        {
            player2Join.gameObject.SetActive(false);
            player2Selection.gameObject.SetActive(true);
            SelectedIndexPlayer2();
            player2IsHere = true;
        }
        // Player 1 join the game
        if (Input.GetButtonDown(Start_P3) && !player3IsHere)
        {
            player3Join.gameObject.SetActive(false);
            player3Selection.gameObject.SetActive(true);
            SelectedIndexPlayer3();
            player3IsHere = true;
        }
        // Player 1 join the game
        if (Input.GetButtonDown(Start_P4) && !player4IsHere)
        {
            player4Join.gameObject.SetActive(false);
            player4Selection.gameObject.SetActive(true);
            SelectedIndexPlayer4();
            player4IsHere = true;
        }
    }

    public void SelectionAndDeselection()
    {
        // Player 1 validate his choice
        if (Input.GetButtonDown(Jump_P1) && !player1Choosed)
        {
            player1Choosed = true;
            championsSelected_.playerSelection.Insert(0, selectionIndexPlayer1);
            championsSelected_.PlayerNumber++;
            player1Selection.gameObject.SetActive(false);
            player1Validate.gameObject.SetActive(true);
        }
        // Player 1 want to change his champion
        if (Input.GetButtonDown(Dodge_P1) && player1Choosed)
        {
            player1Choosed = false;
            championsSelected_.playerSelection.Insert(0, 0);
            championsSelected_.PlayerNumber--;
            player1Selection.gameObject.SetActive(true);
            player1Validate.gameObject.SetActive(false);
        }

        // Player 2 validate his choice
        if (Input.GetButtonDown(Jump_P2) && !player2Choosed)
        {
            player2Choosed = true;
            championsSelected_.playerSelection.Insert(1, selectionIndexPlayer2);
            championsSelected_.PlayerNumber++;
            player2Selection.gameObject.SetActive(false);
            player2Validate.gameObject.SetActive(true);
        }
        // Player 2 want to change his champion
        if (Input.GetButtonDown(Dodge_P2) && player2Choosed)
        {
            player2Choosed = false;
            championsSelected_.playerSelection.Insert(1, 0);
            championsSelected_.PlayerNumber--;
            player2Selection.gameObject.SetActive(true);
            player2Validate.gameObject.SetActive(false);
        }

        // Player 3 validate his choice
        if (Input.GetButtonDown(Jump_P3) && !player3Choosed)
        {
            player3Choosed = true;
            championsSelected_.playerSelection.Insert(2, selectionIndexPlayer3);
            championsSelected_.PlayerNumber++;
            player3Selection.gameObject.SetActive(false);
            player3Validate.gameObject.SetActive(true);
        }
        // Player 3 want to change his champion
        if (Input.GetButtonDown(Dodge_P3) && player3Choosed)
        {
            player3Choosed = false;
            championsSelected_.playerSelection.Insert(2, 0);
            championsSelected_.PlayerNumber--;
            player3Selection.gameObject.SetActive(true);
            player3Validate.gameObject.SetActive(false);
        }

        // Player 4 validate his choice
        if (Input.GetButtonDown(Jump_P4) && !player4Choosed)
        {
            player4Choosed = true;
            championsSelected_.playerSelection.Insert(3, selectionIndexPlayer4);
            championsSelected_.PlayerNumber++;
            player4Selection.gameObject.SetActive(false);
            player4Validate.gameObject.SetActive(true);
        }
        // Player 4 want to change his champion
        if (Input.GetButtonDown(Dodge_P4) && player4Choosed)
        {
            player4Choosed = false;
            championsSelected_.playerSelection.Insert(3, 0);
            championsSelected_.PlayerNumber--;
            player4Selection.gameObject.SetActive(true);
            player4Validate.gameObject.SetActive(false);
        }
    }

    public void SelectionCharacterwithController1()
    {
        if (!m_isAxisOneInUse)
        {
            if (Input.GetAxisRaw(Horizontal_P1) == -1)
            {
                m_isAxisOneInUse = true;
                DecreaseIndex(1);
                SelectedIndexPlayer1();
            }
            else
            {
                if (Input.GetAxisRaw(Horizontal_P1) == 1)
                {
                    m_isAxisOneInUse = true;
                    IncreaseIndex(1);
                    SelectedIndexPlayer1();
                }
            }
        }

        if (Input.GetAxisRaw(Horizontal_P1) == 0 && m_isAxisOneInUse == true)
        {
            m_isAxisOneInUse = false;
        }
    }

    public void SelectionCharacterwithController2()
    {
        if (!m_isAxisTwoInUse)
        {
            if (Input.GetAxisRaw(Horizontal_P2) == -1)
            {
                m_isAxisTwoInUse = true;
                DecreaseIndex(2);
                SelectedIndexPlayer2();
            }
            else
            {
                if (Input.GetAxisRaw(Horizontal_P2) == 1)
                {
                    m_isAxisTwoInUse = true;
                    IncreaseIndex(2);
                    SelectedIndexPlayer2();
                }
            }
        }

        if (Input.GetAxisRaw(Horizontal_P2) == 0 && m_isAxisTwoInUse == true)
        {
            m_isAxisTwoInUse = false;
        }
    }

    public void SelectionCharacterwithController3()
    {
        if (!m_isAxisThreeInUse)
        {
            if (Input.GetAxisRaw(Horizontal_P3) == -1)
            {
                m_isAxisThreeInUse = true;
                DecreaseIndex(3);
                SelectedIndexPlayer2();
            }
            else
            {
                if (Input.GetAxisRaw(Horizontal_P3) == 1)
                {
                    m_isAxisThreeInUse = true;
                    IncreaseIndex(3);
                    SelectedIndexPlayer3();
                }
            }
        }

        if (Input.GetAxisRaw(Horizontal_P3) == 0 && m_isAxisThreeInUse == true)
        {
            m_isAxisThreeInUse = false;
        }
    }

    public void SelectionCharacterwithController4()
    {
        if (!m_isAxisFourInUse)
        {
            if (Input.GetAxisRaw(Horizontal_P4) == -1)
            {
                m_isAxisThreeInUse = true;
                DecreaseIndex(4);
                SelectedIndexPlayer4();
            }
            else
            {
                if (Input.GetAxisRaw(Horizontal_P3) == 1)
                {
                    m_isAxisFourInUse = true;
                    IncreaseIndex(4);
                    SelectedIndexPlayer4();
                }
            }
        }

        if (Input.GetAxisRaw(Horizontal_P4) == 0 && m_isAxisFourInUse == true)
        {
            m_isAxisFourInUse = false;
        }
    }

    public void SelectedIndexPlayer1()
    {
        switch (selectionIndexPlayer1)
        {
            case 1:
                player1KnightButton.GetComponent<Button>().Select();
                KnightPlayer1.gameObject.SetActive(true);
                SorcererPlayer1.gameObject.SetActive(false);
                ArcherPlayer1.gameObject.SetActive(false);
                break;
            case 2:
                player1SorcererButton.GetComponent<Button>().Select();
                KnightPlayer1.gameObject.SetActive(false);
                SorcererPlayer1.gameObject.SetActive(true);
                ArcherPlayer1.gameObject.SetActive(false);
                break;
            case 3:
                player1ArcherButton.GetComponent<Button>().Select();
                KnightPlayer1.gameObject.SetActive(false);
                SorcererPlayer1.gameObject.SetActive(false);
                ArcherPlayer1.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    public void SelectedIndexPlayer2()
    {
        switch (selectionIndexPlayer2)
        {
            case 0:
                player2KnightButton.GetComponent<Button>().Select();
                KnightPlayer2.gameObject.SetActive(true);
                SorcererPlayer2.gameObject.SetActive(false);
                ArcherPlayer2.gameObject.SetActive(false);
                break;
            case 1:
                player2SorcererButton.GetComponent<Button>().Select();
                KnightPlayer2.gameObject.SetActive(false);
                SorcererPlayer2.gameObject.SetActive(true);
                ArcherPlayer2.gameObject.SetActive(false);
                break;
            case 2:
                player2ArcherButton.GetComponent<Button>().Select();
                KnightPlayer2.gameObject.SetActive(false);
                SorcererPlayer2.gameObject.SetActive(false);
                ArcherPlayer2.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    public void SelectedIndexPlayer3()
    {
        switch (selectionIndexPlayer3)
        {
            case 0:
                player3KnightButton.GetComponent<Button>().Select();
                KnightPlayer3.gameObject.SetActive(true);
                SorcererPlayer3.gameObject.SetActive(false);
                ArcherPlayer3.gameObject.SetActive(false);
                break;
            case 1:
                player3SorcererButton.GetComponent<Button>().Select();
                KnightPlayer3.gameObject.SetActive(false);
                SorcererPlayer3.gameObject.SetActive(true);
                ArcherPlayer3.gameObject.SetActive(false);
                break;
            case 2:
                player1ArcherButton.GetComponent<Button>().Select();
                KnightPlayer3.gameObject.SetActive(false);
                SorcererPlayer3.gameObject.SetActive(false);
                ArcherPlayer3.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    public void SelectedIndexPlayer4()
    {
        switch (selectionIndexPlayer4)
        {
            case 0:
                player4KnightButton.GetComponent<Button>().Select();
                KnightPlayer4.gameObject.SetActive(true);
                SorcererPlayer4.gameObject.SetActive(false);
                ArcherPlayer4.gameObject.SetActive(false);
                break;
            case 1:
                player4SorcererButton.GetComponent<Button>().Select();
                KnightPlayer4.gameObject.SetActive(false);
                SorcererPlayer4.gameObject.SetActive(true);
                ArcherPlayer4.gameObject.SetActive(false);
                break;
            case 2:
                player4ArcherButton.GetComponent<Button>().Select();
                KnightPlayer4.gameObject.SetActive(false);
                SorcererPlayer4.gameObject.SetActive(false);
                ArcherPlayer4.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }

    public void DecreaseIndex(int player)
    {
        switch (player)
        {
            case 1:
                if (selectionIndexPlayer1 > 0) selectionIndexPlayer1--;
                break;
            case 2:
                if (selectionIndexPlayer2 > 0) selectionIndexPlayer2--;
                break;
            case 3:
                if (selectionIndexPlayer3 > 0) selectionIndexPlayer3--;
                break;
            case 4:
                if (selectionIndexPlayer4 > 0) selectionIndexPlayer4--;
                break;
            default:
                print("Incorrect");
                break;
        }
    }

    public void IncreaseIndex(int player)
    {
        switch (player)
        {
            case 1:
                if (selectionIndexPlayer1 < 3) selectionIndexPlayer1++;
                break;
            case 2:
                if (selectionIndexPlayer2 < 3) selectionIndexPlayer2++;
                break;
            case 3:
                if (selectionIndexPlayer3 < 3) selectionIndexPlayer3++;
                break;
            case 4:
                if (selectionIndexPlayer4 < 3) selectionIndexPlayer4++;
                break;
            default:
                print("Incorrect");
                break;
        }
    }
}
