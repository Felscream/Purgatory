using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPlayer : MonoBehaviour {
    
    // Default index of the buttonplayer1/2/3/4
    private int selectionIndexPlayer1 = 0;
    private int selectionIndexPlayer2 = 0;
    private int selectionIndexPlayer3 = 0;
    private int selectionIndexPlayer4 = 0;

    public string Jump_P1, Jump_P2, Jump_P3, Jump_P4, Horizontal_P1, Horizontal_P2, Horizontal_P3, Horizontal_P4;
    private RectTransform player1Selection, player2Selection, player3Selection, player4Selection;
    private RectTransform player1Validate, player2Validate, player3Validate, player4Validate;
    private RectTransform player1Join, player2Join, player3Join, player4Join;

    private bool player1IsHere = false, player2IsHere = false, player3IsHere = false, player4IsHere = false;
    private bool player1Choosed = false;

    [SerializeField] GameObject player1ArcherButton;
    [SerializeField] GameObject player1SorcererButton;

    [SerializeField] Transform player2ArcherButton;
    [SerializeField] Transform player2SorcererButton;

    [SerializeField] Transform player3ArcherButton;
    [SerializeField] Transform player3SorcererButton;

    [SerializeField] Transform player4ArcherButton;
    [SerializeField] Transform player4SorcererButton;

    [SerializeField] Transform quitter;
    [SerializeField] Transform back;

    [SerializeField] Transform LaunchGame;

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
    }
	
	// Update is called once per frame
	void Update () {

        // Ajoute un joueur lorsqu'une manette appuie sur le bouton start, si il n'existe pas déjà
        if (Input.GetButtonDown(Jump_P2) && !player1IsHere)
        {
            player1Join.gameObject.SetActive(false);
            player1Selection.gameObject.SetActive(true);
            SelectedIndexPlayer1();
            player1IsHere = true;
        }

        if (player1IsHere)
        {
            if (Input.GetAxisRaw(Horizontal_P2) < 0) {
                DecreaseIndex(1);
                SelectedIndexPlayer1();
            }
            else
            {
                if (Input.GetAxisRaw(Horizontal_P2) > 0)
                {
                    IncreaseIndex(1);
                    SelectedIndexPlayer1();
                }
            }
        }

        /*if (Input.GetButtonDown(Jump_P2) && player1Selection.gameObject.active)
        {
            player1Selection.gameObject.SetActive(false);
            player1Validate.gameObject.SetActive(true);
        }*/
    }

    public void SelectedIndexPlayer1()
    {
        switch (selectionIndexPlayer1)
        {
            case 0:
                player1ArcherButton.GetComponent<Button>().Select();
                break;
            case 1:
                player1SorcererButton.GetComponent<Button>().Select();
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
}
