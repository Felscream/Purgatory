using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSaving : PowerUp {

    [SerializeField] private float staminaCostReductionMultiplier = 0.5f;
	
    public float StaminaCostReductionMultiplier
    {
        get{
            return staminaCostReductionMultiplier;
        }
    }
}
