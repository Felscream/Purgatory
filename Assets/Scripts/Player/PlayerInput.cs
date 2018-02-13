using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private float movementX, movementY;
    private Champion fighter;

    public string Horizontal;
    public string Vertical;
    public string Jump;
    public string Dodge;
    public string PrimaryAttack;
    public string SecondaryAttack;
    public string PowerUp;
    public string Guard;

    private void Awake()
    {
        fighter = GetComponentInChildren<Champion>();
    }
    // Update is called once per frame
    private void Start()
    {
        GetPlayerControlInput();
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
        
        if(fighter != null)
        {
            //MOVEMENT
            fighter.SetHorizontalCtrl(Horizontal);
            fighter.SetVerticalCtrl(Vertical);

            //JUMP
            fighter.SetJumpButton(Jump);

            //DODGE
            fighter.SetDodgeButton(Dodge);

            //PRIMARYATTACK
            fighter.SetPrimaryAttackButton(PrimaryAttack);

            //SECONDARYATTACK
            fighter.SetSecondaryAttackButton(SecondaryAttack);

            //POWERUP
            fighter.SetPowerUpButton(PowerUp);

            //GUARD
            fighter.SetGuardButton(Guard);
        }
    }
}
