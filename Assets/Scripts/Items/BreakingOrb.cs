using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingOrb : MonoBehaviour {

	public int life;
	private Animator anim;
	private Rigidbody2D rgb;
	private Collider2D coll;
	// Use this for initialization
	void Start () {
		life= Random.Range(10,15);
		anim = GetComponent<Animator>();
		rgb = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();

	}
	
	// Update is called once per frame
	void Update () {
		if (life < 10 && life >= 5) {
			anim.SetBool ("Stage2", true);
			//Debug.Log ("Palier degats 1");			
		}
		if (life < 5 && life > 0) {
			anim.SetBool ("Stage3", true);
			//Debug.Log ("Palier degats 2");
		}
	}

	public void TakeDamage(int dmg)
	{
		life -= dmg;
		if (life <= 0) {
			Destroy(rgb);
			coll.isTrigger = true;
			this.gameObject.layer = 16;
			Debug.Log ("layer :" + this.gameObject.layer);
		}
	}
}