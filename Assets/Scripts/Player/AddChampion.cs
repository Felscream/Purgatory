using System.Collections;
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
	private int player1_indexSelection, player2_indexSelection, player3_indexSelection, player4_indexSelection;
	public CanvasGroup[] HUDPlayer = new CanvasGroup[4];
    private ManagerInGame gameManager;
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
		if (championsSelected_ != null)
		{
			player1_indexSelection = championsSelected_.playerSelection[0];
			player2_indexSelection = championsSelected_.playerSelection[1];
			player3_indexSelection = championsSelected_.playerSelection[2];
			player4_indexSelection = championsSelected_.playerSelection[3];

			CreateChampion(player1, player1_indexSelection, HUDPlayer1, 1);
			CreateChampion(player2, player2_indexSelection, HUDPlayer2, 2);
			CreateChampion(player3, player3_indexSelection, HUDPlayer3, 3);
			CreateChampion(player4, player4_indexSelection, HUDPlayer4, 4);
		}
		else
		{        
			// Si on ne passe pas par LobbyManager
			CreateChampion(player1, 1, HUDPlayer1, 1);
			CreateChampion(player2, 3, HUDPlayer2, 2);
			CreateChampion(player3, 2, HUDPlayer3, 3);
			CreateChampion(player4, 1, HUDPlayer4, 4);
		}

        StartCoroutine(ManagerInGame.GetInstance().StartGame());
	}

	// Ajoute un joueur de la classe correspondante, si il n'existe pas déjà
	public void CreateChampion(GameObject player, int index, CanvasGroup HUDPlayer, int controllerNumber)
	{
		if (index != 0)
		{
			player.SetActive(true);
		}

		if (!player.GetComponent<PlayerInput>().ObjectsInstantiated())
		{
			Champion championPlayer = null;
			switch (index)
			{
			case 1: // Knight selected
				championPlayer = Instantiate(prefabKnight, player.transform, false); // A MODIFIER avec le perso choisit par le joueur
				break;
			case 2: // sorcerer selected
				championPlayer = Instantiate(prefabSorcerer, player.transform, false); // A MODIFIER avec le perso choisit par le joueur
				break;
			case 3: // archer selected
				championPlayer = Instantiate(prefabArcher, player.transform, false); // A MODIFIER avec le perso choisit par le joueur
				break;
			default: // if 0, nothing selected
				break;
			}
			if (championPlayer != null)
			{

				championPlayer.transform.localPosition = new Vector3(0, 0, 0);
				// On lui donne le HUD correspondant
				championPlayer.playerHUD = HUDPlayer;
				// On redonne les input au Champion créé
				player.GetComponentInChildren<FollowPlayerScript>().SetChampion(championPlayer);
				player.GetComponent<PlayerInput>().SetChampion(championPlayer);
				player.GetComponent<PlayerInput>().GetPlayerControlInput();
                /*Debug.Log(ControllerManager.Instance.GetController(index).IsConnected);
                if(ControllerManager.Instance.GetController(index).IsConnected)
                {*/

                championPlayer.SetController(controllerNumber);
                //}
                
			}
		}
	}
}