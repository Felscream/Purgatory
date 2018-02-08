using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private float movementX, movementY;
    private Champion fighter;

    public string Horizontal;
    public string Jump;
    public string Dodge;
    public string PrimaryAttack;
    public string PowerUp;
    private void Awake()
    {
        fighter = GetComponentInChildren<Champion>();
    }
    // Update is called once per frame
    private void Start()
    {
        GetPlayerControlInput();

    }

    private void FixedUpdate()
    {
        
    }

    private bool ObjectsInstantiated()
    {
        if(fighter != null)
            return true;
        return false;
    }

    

    
    /**************************
    ****     GET INPUT      ***
    ***************************/
    public void GetPlayerControlInput()
    {
        //MOVEMENT
        if(fighter != null)
        {
            fighter.SetHorizontalCtrl(Horizontal);
        }

        //JUMP
        if (fighter != null)
        {
            fighter.SetJumpButton(Jump);
        }

        //DODGE
        if (fighter != null)
        {
            fighter.SetDodgeButton(Dodge);
        }

        //PRIMARYATTACK
        if (fighter != null)
        {
            fighter.SetPrimaryAttackButton(PrimaryAttack);
        }

        //POWERUP
        if (fighter != null)
        {
            fighter.SetPowerUpButton(PowerUp);
        }
    }
}
