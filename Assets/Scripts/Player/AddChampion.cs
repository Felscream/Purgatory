﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddChampion : MonoBehaviour {

    private GameObject player1, player2, player3, player4;
    private GameObject floatingJ1, floatingJ2, floatingJ3, floatingJ4;
    public CanvasGroup HUDPlayer1, HUDPlayer2, HUDPlayer3, HUDPlayer4;
    public Champion prefabKnight, prefabSorcerer, prefabArcher;
    public string Start_P1, Start_P2, Start_P3, Start_P4;

    // ChampionsSelected instance from player selection
    ChampionsSelected championsSelected_;
    private int player1_indexSelection = 0, player2_indexSelection = 0, player3_indexSelection = 0, player4_indexSelection = 0;

    // Use this for initialization
    void Start ()
    {
        player1 = this.transform.GetChild(0).gameObject; // Player1 doit être le premier enfant de players
        player2 = this.transform.GetChild(1).gameObject; // Player2 doit être le deuxième enfant de players
        player3 = this.transform.GetChild(2).gameObject; // Player3 doit être le troisième enfant de players
        player4 = this.transform.GetChild(3).gameObject; // Player4 doit être le quatrième enfant de players

        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);

        // On prend les J1/ J2/ J3/ J4 flottants 
        floatingJ1 = player1.transform.GetChild(0).gameObject;   
        floatingJ2 = player2.transform.GetChild(0).gameObject;
        floatingJ3 = player3.transform.GetChild(0).gameObject;
        floatingJ4 = player4.transform.GetChild(0).gameObject;

        // Instance for prefab
        championsSelected_ = ChampionsSelected.GetInstance();
        if(championsSelected_ != null)
        {
            switch (championsSelected_.PlayerNumber)
            {
                case 0:
                    player1_indexSelection = championsSelected_.Player1_indexSelection;
                    break;
                case 1:
                    player1_indexSelection = championsSelected_.Player1_indexSelection;
                    player2_indexSelection = championsSelected_.Player2_indexSelection;
                    break;
                case 2:
                    player1_indexSelection = championsSelected_.Player1_indexSelection;
                    player2_indexSelection = championsSelected_.Player2_indexSelection;
                    player3_indexSelection = championsSelected_.Player3_indexSelection;
                    break;
                case 3:
                    player1_indexSelection = championsSelected_.Player1_indexSelection;
                    player2_indexSelection = championsSelected_.Player2_indexSelection;
                    player3_indexSelection = championsSelected_.Player3_indexSelection;
                    player4_indexSelection = championsSelected_.Player4_indexSelection;
                    break;
                default:
                    break;
            }
        }
        else
        {
            player1_indexSelection = 0;
            player2_indexSelection = 1;
            player3_indexSelection = 2;
            player4_indexSelection = 1;
        }
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Ajoute un joueur lorsqu'une manette appuie sur le bouton start, si il n'existe pas déjà
        if (!player1.activeSelf)
        {
            player1.SetActive(true);
            if (!player1.GetComponent<PlayerInput>().ObjectsInstantiated())
            {
                Champion championPlayer1;
                switch (player1_indexSelection)
                {
                    case 0:
                        championPlayer1 = Instantiate(prefabKnight, player1.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 1:
                        championPlayer1 = Instantiate(prefabSorcerer, player1.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 2:
                        championPlayer1 = Instantiate(prefabArcher, player1.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    default:
                        championPlayer1 = Instantiate(prefabKnight, player1.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                }
                championPlayer1.transform.localPosition = new Vector3(0, 0, 0);
                // On lui donne le HUD correspondant
                championPlayer1.playerHUD = HUDPlayer1;
                // On redonne les input au Champion créé
                player1.GetComponentInChildren<FollowPlayerScript>().SetChampion(championPlayer1);
                player1.GetComponent<PlayerInput>().SetChampion(championPlayer1);
                player1.GetComponent<PlayerInput>().GetPlayerControlInput();
            }
        }
        if (!player2.activeSelf)
        {
            player2.SetActive(true);
            if (!player2.GetComponent<PlayerInput>().ObjectsInstantiated())
            {
                Champion championPlayer2;
                switch (player2_indexSelection)
                {
                    case 0:
                        championPlayer2 = Instantiate(prefabKnight, player2.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 1:
                        championPlayer2 = Instantiate(prefabSorcerer, player2.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 2:
                        championPlayer2 = Instantiate(prefabArcher, player2.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    default:
                        championPlayer2 = Instantiate(prefabKnight, player2.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                }
                championPlayer2.transform.localPosition = new Vector3(0, 0, 0);
                championPlayer2.playerHUD = HUDPlayer2;
                player2.GetComponentInChildren<FollowPlayerScript>().SetChampion(championPlayer2);
                player2.GetComponent<PlayerInput>().SetChampion(championPlayer2);
                player2.GetComponent<PlayerInput>().GetPlayerControlInput();
            }
        }
        if (Input.GetButtonDown(Start_P3) && !player3.activeSelf)
        {
            player3.SetActive(true);
            if (!player3.GetComponent<PlayerInput>().ObjectsInstantiated())
            {
                Champion championPlayer3;
                switch (player3_indexSelection)
                {
                    case 0:
                        championPlayer3 = Instantiate(prefabKnight, player3.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 1:
                        championPlayer3 = Instantiate(prefabSorcerer, player3.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 2:
                        championPlayer3 = Instantiate(prefabArcher, player3.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    default:
                        championPlayer3 = Instantiate(prefabKnight, player3.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                }
                championPlayer3.transform.localPosition = new Vector3(0, 0, 0);
                championPlayer3.playerHUD = HUDPlayer3;
                player3.GetComponentInChildren<FollowPlayerScript>().SetChampion(championPlayer3);
                player3.GetComponent<PlayerInput>().SetChampion(championPlayer3);
                player3.GetComponent<PlayerInput>().GetPlayerControlInput();
            }
        }
        if (Input.GetButtonDown(Start_P4) && !player4.activeSelf)
        {
            player4.SetActive(true);
            if (!player4.GetComponent<PlayerInput>().ObjectsInstantiated())
            {
                Champion championPlayer4;
                switch (player4_indexSelection)
                {
                    case 0:
                        championPlayer4 = Instantiate(prefabKnight, player4.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 1:
                        championPlayer4 = Instantiate(prefabSorcerer, player4.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    case 2:
                        championPlayer4 = Instantiate(prefabArcher, player4.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                    default:
                        championPlayer4 = Instantiate(prefabKnight, player4.transform, false); // A MODIFIER avec le perso choisit par le joueur
                        break;
                }
                championPlayer4.transform.localPosition = new Vector3(0, 0, 0);
                championPlayer4.playerHUD = HUDPlayer4;
                player4.GetComponentInChildren<FollowPlayerScript>().SetChampion(championPlayer4);
                player4.GetComponent<PlayerInput>().SetChampion(championPlayer4);
                player4.GetComponent<PlayerInput>().GetPlayerControlInput();
            }
        }
    }
}
