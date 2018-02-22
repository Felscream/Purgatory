using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickUp : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.tag.Equals("Pick Up"))
		{
			Debug.Log ("oui");
			other.gameObject.SetActive (false);
		}
	}
}

