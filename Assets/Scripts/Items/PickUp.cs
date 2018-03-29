using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	private Champion player;
	public int HealthAmount = 20;
	public int StaminaAmount = 10;
	public int LimitBreakAmount = 70;
	private int countStamina = 0;
	private int countLimitBreak =0;

	void Awake(){
		player = GetComponentInChildren<Champion> ();
	}
	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.layer.Equals (16))
			{			//16: layer collectables
			if (other.gameObject.tag.Equals ("Health")) {
				player.Health += HealthAmount;
				Debug.Log ("Health: " + player.Health);
			}
			if (other.gameObject.tag.Equals ("Stamina")) {
				InvokeRepeating ("StaminaRecover", 1, 1);
				Debug.Log ("Stamina: " + player.Stamina);
			}
			if (other.gameObject.tag.Equals ("BreakingOrb")) {
				//InvokeRepeating("LimitBreakUp",1,1);
				player.LimitBreakGauge=LimitBreakAmount;
				player.Health = player.BaseHealth;
				Debug.Log ("Ajout des buffs ouroboros");
			}
			other.gameObject.SetActive (false);
		}
	}

	void StaminaRecover (){
		if (!player.Dead) {
			player.staminaRegenerationPerSecond = 25f;
			//Debug.Log ("Stamina: " + player.staminaRegenerationPerSecond);
			countStamina += 1;
			if (StaminaAmount == countStamina) {
				Debug.Log ("End Recovery");
				CancelInvoke ("StaminaRecover");
				countStamina = 0;
				player.staminaRegenerationPerSecond = 15f;
			}
		}
	}
	 /*
	void LimitBreakUp(){
		if (!player.Dead) {
			player.IncreaseLimitBreak (UltiAmount);
			player.Health = player.BaseHealth;
			countLimitBreak += 1;
			if (LimitBreakAmount == countLimitBreak) {
				Debug.Log ("End Recovery");
				CancelInvoke ("LimitBreakUp");
				countLimitBreak = 0;
				player.limitBreakPerSecond = 0.4f;
			}
		}
	}*/
}