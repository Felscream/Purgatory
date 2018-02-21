﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Champion
{
    [Header("ProjectileSettingArrow")]
    [SerializeField] protected GameObject arrow;
    [SerializeField] protected Vector2 projectileSpawnOffsetArrow;

    // Tenir bouton enfoncé
    protected float keyTimer = 0.0f; //Our timer
    protected float keyLength= 5.0f; //The duration needed to trigger the function
    protected float forceArrow = 0.3f; //The force of the arrow
    protected bool isKeyActive = false;


    protected override void LateUpdate()
    {
        base.LateUpdate();

    }
    protected override void PrimaryAttack()
    {
        base.PrimaryAttack();
    }

    protected override void Update()
    {
        /*
        //Initial key press
        if (Input.GetButtonDown(SecondaryAttackButton) && !isKeyActive && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard)
        {
            //Get the timestamp
            keyTimer = Time.time;

        }
        //Key released
        //This will not execute if the button is held (isKeyActive is false)
        if (Input.GetButtonUp(SecondaryAttackButton) && isKeyActive)
        {
            forceArrow = (Time.time - keyTimer);
            SecondaryAttack();
        }*/

        base.Update();
    }


    public void SpawnArrow()
    {
        if (powerUp is SpecialArrowEffect)
        {
            SpecialArrowEffect temp = (SpecialArrowEffect)powerUp;

            if (powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
            {
                temp.getRandomPowerUp();
            }
            else
            {
                temp.getNoPowerUp();
            }

            Vector2 SpawnPoint = new Vector2(transform.position.x + projectileSpawnOffsetArrow.x * facing, transform.position.y + projectileSpawnOffsetArrow.y);
            GameObject arrow1 = Instantiate(arrow, SpawnPoint, transform.rotation);
            Arrow ar = arrow1.GetComponent<Arrow>();

            if (temp.getPoisonState)
            {
                ar.arrowStatus = Arrow.Enum_ArrowStatus.poison;
            }
            else
            {
                if (temp.getStunState)
                {
                    ar.arrowStatus = Arrow.Enum_ArrowStatus.stun;
                }
                else
                {
                    if (temp.getSlowState)
                    {
                        ar.arrowStatus = Arrow.Enum_ArrowStatus.slow;
                    }
                    else
                    {
                        ar.arrowStatus = Arrow.Enum_ArrowStatus.normal;
                    }
                }
            }
            ar.Owner = this;
            ar.Direction = facing;
            ar.GetComponent<Rigidbody2D>().AddForce(ar.Force * facing);
            rb.gravityScale = 1.0f;
            AllowInputs();
        }
    }

    protected override void CastHitBox(int attackType)
    {
        throw new NotImplementedException();
    }
}