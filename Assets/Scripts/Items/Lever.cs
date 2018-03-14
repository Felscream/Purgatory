﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour {

    public GameObject trap;
    public Sprite secondPositionLever;
    public bool canEngage = true;

    private Sprite firstPositionLever;


    // Use this for initialization
    void Start () {

        firstPositionLever = GetComponent<SpriteRenderer>().sprite;
    }
	

    public void Engage()
    {
        if (canEngage)
        {
            canEngage = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = secondPositionLever;
            trap.GetComponent<Trap>().Engage();
            trap.SetActive(true);
        }
    }

    public void Disengage()
    {
        canEngage = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = firstPositionLever;
        trap.SetActive(false);
    }

}
