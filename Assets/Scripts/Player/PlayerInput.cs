using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private float movementX, movementY;
    private Champion fighter;

    private void Start()
    {
        fighter = GetComponentInChildren<Champion>();
    }
    // Update is called once per frame
    private void Update()
    {
        GetPlayerControlInput();

    }

    private void FixedUpdate()
    {
        //MOVING
        fighter.Move(movementX, movementY);
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
            if(fighter.InputStatus != Enum_InputStatus.blocked)
            {
                movementX = Input.GetAxisRaw("Horizontal");
            }
            else if(fighter.InputStatus == Enum_InputStatus.blocked)
            {
                movementX = 0;
                movementY = 0;
            }
        }
        
    }
}
