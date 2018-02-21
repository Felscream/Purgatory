using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialArrowEffect : PowerUp
{
    [SerializeField] private bool isNormal = true;
    [SerializeField] private bool isStunned = false;
    [SerializeField] private bool isPoisonned = false;
    [SerializeField] private bool isSlowed = false;

    
    public void getNoPowerUp()
    {
        isNormal = true;
        isStunned = false;
        isPoisonned = false;
        isSlowed = false;
    }

    public void getRandomPowerUp()
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                isNormal = false;
                isStunned = true;
                isPoisonned = false;
                isSlowed = false;
                break;
            case 1:
                isNormal = false;
                isStunned = false;
                isPoisonned = true;
                isSlowed = false;
                break;
            case 2:
                isNormal = false;
                isStunned = false;
                isPoisonned = false;
                isSlowed = true;
                break;
            default:
                isNormal = true;
                isStunned = false;
                isPoisonned = false;
                isSlowed = false;
                break;
        }
    }

    public bool getNormalState
    {
        get
        {
            return isNormal;
        }
    }

    public bool getStunState
    {
        get
        {
            return isStunned;
        }
    }

    public bool getPoisonState
    {
        get
        {
            return isPoisonned;
        }
    }

    public bool getSlowState
    {
        get
        {
            return isSlowed;
        }
    }
}
