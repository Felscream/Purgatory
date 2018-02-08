using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickUp : MonoBehaviour {

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Player1"))
		{
			other.gameObject.SetActive (false);
		}
	}
}
