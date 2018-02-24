using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	private Champion player;
	public int addHealth = 20;
	public int addStamina = 20;

	void Awake(){
		player = GetComponentInChildren<Champion> ();
	}
	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.layer.Equals(16))
			//16: layer collectables
		{
			if (other.gameObject.tag.Equals("Health")){
				player.Health += addHealth;
				Debug.Log("Health: " + player.Health);
			}
			if (other.gameObject.tag.Equals ("Stamina")) {
				player.Stamina += addStamina;
				Debug.Log("Health: " + player.Stamina);
			}
			if (other.gameObject.tag.Equals ("Orb")) {
				//ajouter la commande de recharge d'ulti
			}
			other.gameObject.SetActive (false);
		}
	}
}