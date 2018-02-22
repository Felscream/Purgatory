using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerInGame : MonoBehaviour {

	public GameObject Health;
	public GameObject Stamina;
	public GameObject Orb;
	public GameObject Plateform;
	private float Timer = 0.0f;
	private bool SpawnHealth;
	private bool SpawnStamina;
	private bool SpawnOrb;
	private PlayerHealthSlider _playerhealth;
	public Champion player1;
	public Champion player2;
	public Champion player3;
	public Champion player4;
	public bool playerAlive1 = false;
	public bool playerAlive2 = false;
	public bool playerAlive3 = false;
	public bool playerAlive4 = false;
	//private bool SpawnPlateform;
	private int PlayerAlive=0;

	void Awake(){
		if (player1 != null) {
			PlayerAlive+=1 ;
			playerAlive1 = true;
		}
		if (player2 != null) {
			PlayerAlive+=1 ;
			playerAlive2 = true;
		}
		if (player3 != null) {
			PlayerAlive+=1 ;
			playerAlive3 = true;
		}
		if (player4 != null) {
			PlayerAlive+=1 ;
			playerAlive4 = true;
		}
		Debug.Log(PlayerAlive);
	}
	void Update () {
		Timer += Time.deltaTime;
		//Debug.Log (player1.Health);
		//Debug.Log (player2.Health);
		//Apparition des items sur la map à partir des prefabs
		if (Timer >= 5 && !SpawnHealth) {
			Instantiate (Health);
			SpawnHealth = true;
		}

		if (Timer >= 5 && !SpawnOrb) {
			Instantiate (Orb);
			//Instantiate (Plateform);
			SpawnOrb = true;
		}

		if (Timer >= 5 && !SpawnStamina) {
			Instantiate (Stamina);
			SpawnStamina = true;
		}

		if (player1 != null)
		{
			if (player1.Health <= 0 && playerAlive1) {
				PlayerAlive -= 1;
				playerAlive1 = false;
			}
		}

		if (player2 != null)
		{
			if (player2.Health <= 0 && playerAlive2) {
				PlayerAlive -= 1;
				playerAlive2 = false;
			}
		}

		if (player3 != null)
		{
			if (player3.Health <= 0 && playerAlive3) {
				PlayerAlive -= 1;
				playerAlive3 = false;
			}
		}

		if (player4 != null)
		{
			if (player4.Health <= 0 && playerAlive4) {
				PlayerAlive -= 1;
				playerAlive4 = false;
			}
		}


		if (PlayerAlive == 1) {
			SceneManager.LoadScene (2);
			//Ajout de la suite
		}

		Debug.Log (PlayerAlive);
	}
}
