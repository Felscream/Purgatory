﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class VictoryMenu: MonoBehaviour {
    // ChampionsSelected instance
    ChampionsSelected championsSelected_;
    private int player1_indexSelection = 0, player2_indexSelection = 0, player3_indexSelection = 0, player4_indexSelection = 0;

    // Calculate score
    public Image ouroborosCooldown;

    // Use this for initialization
    void Start () {
        // Instance for prefab
        championsSelected_ = ChampionsSelected.GetInstance();
    }
	
	// Update is called once per frame
	void Update () {
        while (ouroborosCooldown.fillAmount > 0)
        {
            //Reduce fill amount over 30 seconds
            ouroborosCooldown.fillAmount -= 0.5f * Time.deltaTime;
        }
        CalculateScoreImageRefill();
    }

    public void CalculateScoreImageRefill()
    {
        ouroborosCooldown.fillAmount = 1.0f;
    }
}