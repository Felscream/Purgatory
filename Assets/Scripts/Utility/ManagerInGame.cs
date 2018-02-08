using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInGame : MonoBehaviour {

	public GameObject Health;
	public GameObject Stamina;
	public GameObject Orb;
	public GameObject Plateform;
	private float Timer = 0.0f;
	private bool SpawnHealth;
	private bool SpawnStamina;
	private bool SpawnOrb;
	//private bool SpawnPlateform;
	//private int PlayerAlive;

	void Update () {
		Timer += Time.deltaTime;

		//Apparition des items sur la map à partir des prefabs
		if (Timer >= 5 && !SpawnHealth) {
			Instantiate (Health);
			SpawnHealth = true;
		}

		if (Timer >= 20 && !SpawnOrb) {
			Instantiate (Orb);
			Instantiate (Plateform);
			SpawnOrb = true;
		}

		if (Timer >= 10 && !SpawnStamina) {
			Instantiate (Stamina);
			SpawnStamina = true;
		}

		/*if (PlayerAlive == 1) {
			
		}*/
	}
}
