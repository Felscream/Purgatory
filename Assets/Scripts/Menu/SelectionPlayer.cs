﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionPlayer : MonoBehaviour {

    // Default index of the buttonplayer1/2/3/4
    private int selectionIndexPlayer1 = 0;
    private int selectionIndexPlayer2 = 0;
    private int selectionIndexPlayer3 = 0;
    private int selectionIndexPlayer4 = 0;
    private int playerNumber = 0;

    public RectTransform KnightPlayer1;
    private Animator KnightPlayer1Anim;
    public RectTransform ArcherPlayer1;
    private Animator ArcherPlayer1Anim;
    public RectTransform SorcererPlayer1;
    private Animator SorcererPlayer1Anim;

    public RectTransform KnightPlayer2;
    private Animator KnightPlayer2Anim;
    public RectTransform ArcherPlayer2;
    private Animator ArcherPlayer2Anim;
    public RectTransform SorcererPlayer2;
    private Animator SorcererPlayer2Anim;

    public RectTransform KnightPlayer3;
    private Animator KnightPlayer3Anim;
    public RectTransform ArcherPlayer3;
    private Animator ArcherPlayer3Anim;
    public RectTransform SorcererPlayer3;
    private Animator SorcererPlayer3Anim;

    public RectTransform KnightPlayer4;
    private Animator KnightPlayer4Anim;
    public RectTransform ArcherPlayer4;
    private Animator ArcherPlayer4Anim;
    public RectTransform SorcererPlayer4;
    private Animator SorcererPlayer4Anim;

    public string Jump_P1, Jump_P2, Jump_P3, Jump_P4, Start_P1, Start_P2, Start_P3, Start_P4, Horizontal_P1, Horizontal_P2, Horizontal_P3, Horizontal_P4;
    private RectTransform player1Selection, player2Selection, player3Selection, player4Selection;
    private RectTransform player1Validate, player2Validate, player3Validate, player4Validate;
    private RectTransform player1Join, player2Join, player3Join, player4Join;

    private bool player1IsHere = false, player2IsHere = false, player3IsHere = false, player4IsHere = false;
    private bool player1Choosed = false, player2Choosed = false, player3Choosed = false, player4Choosed = false;

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

    [SerializeField] Transform quitter;
    [SerializeField] Transform back;

    [SerializeField] Transform LaunchGame;

    // ChampionsSelected instance
    ChampionsSelected championsSelected_;
    bool m_isAxisOneInUse = false, m_isAxisTwoInUse = false, m_isAxisThreeInUse = false, m_isAxisFourInUse = false;

    // Use this for initialization
    void Start () {
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

        KnightPlayer1Anim = KnightPlayer1.GetComponent<Animator>();
        ArcherPlayer1Anim = ArcherPlayer1.GetComponent<Animator>();
        SorcererPlayer1Anim = SorcererPlayer1.GetComponent<Animator>();

        KnightPlayer2Anim = KnightPlayer2.GetComponent<Animator>();
        ArcherPlayer2Anim = ArcherPlayer2.GetComponent<Animator>();
        SorcererPlayer2Anim = SorcererPlayer2.GetComponent<Animator>();

        KnightPlayer3Anim = KnightPlayer3.GetComponent<Animator>();
        ArcherPlayer3Anim = ArcherPlayer3.GetComponent<Animator>();
        SorcererPlayer3Anim = SorcererPlayer3.GetComponent<Animator>();

        KnightPlayer4Anim = KnightPlayer4.GetComponent<Animator>();
        ArcherPlayer4Anim = ArcherPlayer4.GetComponent<Animator>();
        SorcererPlayer4Anim = SorcererPlayer4.GetComponent<Animator>();

        championsSelected_ = ChampionsSelected.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // Player 1 join the game
        if (Input.GetButtonDown(Start_P1) && !player1IsHere)
        {
            player1Join.gameObject.SetActive(false);
            player1Selection.gameObject.SetActive(true);
            SelectedIndexPlayer1();
            player1IsHere = true;
        }
        if (Input.GetButtonDown(Start_P2) && !player2IsHere)
        {
            player2Join.gameObject.SetActive(false);
            player2Selection.gameObject.SetActive(true);
            SelectedIndexPlayer2();
            player2IsHere = true;
        }
        if (Input.GetButtonDown(Start_P3) && !player3IsHere)
        {
            player3Join.gameObject.SetActive(false);
            player3Selection.gameObject.SetActive(true);
            SelectedIndexPlayer3();
            player3IsHere = true;
        }

        if (Input.GetButtonDown(Start_P4) && !player4IsHere)
        {
            player4Join.gameObject.SetActive(false);
            player4Selection.gameObject.SetActive(true);
            SelectedIndexPlayer4();
            player4IsHere = true;
        }

        // Player 1 validate his choice
        if(Input.GetButtonDown(Jump_P1) && !player1Choosed)
        {
            player1Choosed = true;
            championsSelected_.Player1_indexSelection = selectionIndexPlayer1;
        }

        // Player 2 validate his choice
        if (Input.GetButtonDown(Jump_P2) && !player2Choosed)
        {
            player2Choosed = true;
            championsSelected_.Player2_indexSelection = selectionIndexPlayer2;
        }

        // Player 3 validate his choice
        if (Input.GetButtonDown(Jump_P3) && !player3Choosed)
        {
            player3Choosed = true;
            championsSelected_.Player3_indexSelection = selectionIndexPlayer3;
        }

        // Player 4 validate his choice
        if (Input.GetButtonDown(Jump_P4) && !player4Choosed)
        {
            player1Choosed = true;
            championsSelected_.Player4_indexSelection = selectionIndexPlayer4;
        }

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

        if (player1Choosed)
        {
            //KnightPlayer1Anim.SetBool("isActive", true);
            LoadMainScene();
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

        if (Input.GetAxis(Horizontal_P2) > 0 && m_isAxisTwoInUse == false)
        {
            if (Input.GetAxisRaw(Horizontal_P2) == 0)
            {
                m_isAxisTwoInUse = false;
            }
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

        if (Input.GetAxis(Horizontal_P3) > 0 && m_isAxisThreeInUse == false)
        {
            if (Input.GetAxisRaw(Horizontal_P3) == 0)
            {
                m_isAxisThreeInUse = false;
            }
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

        if (Input.GetAxis(Horizontal_P4) > 0 && m_isAxisFourInUse == false)
        {
            if (Input.GetAxisRaw(Horizontal_P4) == 0)
            {
                m_isAxisFourInUse = false;
            }
        }
    }

    public void SelectedIndexPlayer1()
    {
        switch (selectionIndexPlayer1)
        {
            case 0:
                player1KnightButton.GetComponent<Button>().Select();
                KnightPlayer1.gameObject.SetActive(true);
                SorcererPlayer1.gameObject.SetActive(false);
                ArcherPlayer1.gameObject.SetActive(false);
                break;
            case 1:
                player1SorcererButton.GetComponent<Button>().Select();
                KnightPlayer1.gameObject.SetActive(false);
                SorcererPlayer1.gameObject.SetActive(true);
                ArcherPlayer1.gameObject.SetActive(false);
                break;
            case 2:
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
                if(selectionIndexPlayer1 > 0) selectionIndexPlayer1--;
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
                if (selectionIndexPlayer1 < 2) selectionIndexPlayer1++;
                break;
            case 2:
                if (selectionIndexPlayer2 < 2) selectionIndexPlayer2++;
                break;
            case 3:
                if (selectionIndexPlayer3 < 2) selectionIndexPlayer3++;
                break;
            case 4:
                if (selectionIndexPlayer4 < 2) selectionIndexPlayer4++;
                break;
            default:
                print("Incorrect");
                break;
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
