using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingOrb : MonoBehaviour {

	public int maxLife;
	public int life;
	private bool Stage1 = true;
	private bool Stage2 = false;
	private bool Stage3 = false;
	private bool damaged = false;
	private Animator setStage2;
	private Animator setStage3;
	// Use this for initialization
	void Start () {
		maxLife= Random.Range(10,15);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
