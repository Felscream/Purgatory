using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	private Champion player;
	public int HealthAmount = 20;
	public int StaminaAmount = 20;
	private int countHealth = 0;
	private int countStamina = 0;

	void Awake(){
		player = GetComponentInChildren<Champion> ();
	}
	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.layer.Equals(16))
			//16: layer collectables
		{
			if (other.gameObject.tag.Equals("Health")){
				InvokeRepeating ("HealthRecover", 1, 2);
				//player.Health += 20 /*addHealth*/;
				Debug.Log("Health: " + player.Health);
			}
			if (other.gameObject.tag.Equals ("Stamina")) {
				InvokeRepeating ("StaminaRecover", 1, 2);
				Debug.Log("Stamina: " + player.Stamina);
			}
			other.gameObject.SetActive (false);
		}
	}

	void HealthRecover (){
        if (!player.Dead)
        {
            player.Health += 1;
            Debug.Log("Health: " + player.Health);
            countHealth += 1;
            if (HealthAmount == countHealth)
            {
                Debug.Log("End Recovery");
                CancelInvoke("HealthRecover");
                countHealth = 0;
            }
        }
	}

	void StaminaRecover (){
		if (!player.Dead) {
			player.staminaRegenerationPerSecond = 25f;
			Debug.Log ("Stamina: " + player.staminaRegenerationPerSecond);
			countStamina += 1;
			if (StaminaAmount == countStamina) {
				Debug.Log ("End Recovery");
				CancelInvoke ("StaminaRecover");
				countStamina = 0;
				player.staminaRegenerationPerSecond = 15f;
			}
		}
	}
}