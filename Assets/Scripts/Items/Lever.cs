using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    public GameObject trap;
    public float timeToStayEnabled;
    public Sprite secondPositionLever;

    private Sprite firstPositionLever;
    private bool canEngage = true;


    // Use this for initialization
    void Start () {

        firstPositionLever = GetComponent<Sprite>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Engage()
    {
        canEngage = false;
    }

}
