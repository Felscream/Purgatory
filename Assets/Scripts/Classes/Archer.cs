using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Champion
{
    [Header("ProjectileSettingArrow")]
    [SerializeField]
    protected GameObject arrow;
    [SerializeField] protected Vector2 projectileSpawnOffsetArrow;

    // Tenir bouton enfoncé
    protected float keyTimer = 0.0f; //Our timer
    protected float forceArrow = 0.3f; //The force of the arrow
    protected bool isKeyActive = false;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(combo1.offset.x, combo1.offset.y, 0) + transform.position, new Vector3(combo1.size.x, combo1.size.y, 1));
    }

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
        //Initial key press
        /*if (Input.GetButtonDown(SecondaryAttackButton) && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard)
        {
            //Get the timestamp
            keyTimer = Time.time;
            while(Input.GetButtonUp(SecondaryAttackButton) || (Time.time - keyTimer) >= 0.3f)
            {
                // Do nothing
            }
            forceArrow = (Time.time - keyTimer);
            SecondaryAttack();
        }
        
        
        //Key released
        //This will not execute if the button is held (isKeyActive is false)
        if (Input.GetButtonUp(SecondaryAttackButton))
        {
            forceArrow = (Time.time - keyTimer);
            SecondaryAttack();
        }*/

        if (Input.GetButtonDown(SecondaryAttackButton) && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard)
        {
            SecondaryAttack();
        }

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
            float forceArr = forceArrow * facing;
            ar.GetComponent<Rigidbody2D>().AddForce(new Vector2(forceArr, 0));
            rb.gravityScale = 1.0f;
            AllowInputs();
        }
    }

    protected override void CastHitBox(int attackType)
    {
        switch (attackType)
        {
            case 1:
                combo1.CastHitBox();
                break;
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }
}
