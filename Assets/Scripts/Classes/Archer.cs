using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enum_ChargeLevel
{
    low = 0,
    medium = 1,
    high = 2
}

public class Archer : Champion
{
    [Header("ChargeSettings")]
    
    [SerializeField] protected float firstLevelTime = 0.75f;
    [SerializeField] protected float firstLevelMultiplier = 1.5f;
    [SerializeField] protected float secondLevelTime = 1.5f;
    [SerializeField] protected float secondLevelMultiplier = 2.5f;
    [SerializeField] protected float speedReductionMultiplier = 0.75f;

    [Header("ProjectileSettingArrow")]
    [SerializeField] protected GameObject arrow;
    [SerializeField] protected Vector2 projectileSpawnOffsetArrow;

    // Tenir bouton enfoncé
    protected float baseChargeMultiplier = 1.0f;
    protected float chargeTimer = 0.0f; //Our timer
    protected float forceArrow = 0.3f; //The force of the arrow
    protected bool isKeyActive = false;
    protected Enum_ChargeLevel chargeLevel = Enum_ChargeLevel.low;
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(combo1.offset.x, combo1.offset.y, 0) + transform.position, new Vector3(combo1.size.x, combo1.size.y, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(projectileSpawnOffsetArrow.x, projectileSpawnOffsetArrow.y, 0) + transform.position, 0.1f);
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

        

        base.Update();
        
        if (Input.GetButton(SecondaryAttackButton) && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard && IsGrounded() && !dead)
        {
            if (Input.GetButtonDown(SecondaryAttackButton))
            {
                ReduceStamina(secondaryFireStaminaCost);
            }
            Charge();
        }
        else if (Input.GetButtonUp(SecondaryAttackButton))
        {
            Release();
        }
    }

    protected override void SecondaryAttack()
    {
        //nothing
    }

    private void Charge()
    {
        if(chargeTimer == 0)
        {
            animator.SetTrigger("Draw");
        }
        chargeTimer += Time.deltaTime;
        if (chargeTimer > 0)
        {
            animator.SetBool("Hold", true);
            speed = baseSpeed * 0.75f;
        }
        if(chargeTimer >= secondLevelTime)
        {
            chargeLevel = Enum_ChargeLevel.high;
        }
        else if(chargeTimer >= firstLevelTime)
        {
            chargeLevel = Enum_ChargeLevel.medium;
        }
        else
        {
            chargeLevel = Enum_ChargeLevel.low;
        }
        Debug.Log(chargeLevel +" "+ chargeTimer);
    }

    private void Release()
    {
        animator.SetBool("Hold", false);
        speed = baseSpeed;
        chargeTimer = 0.0f;
    }

    public void SpawnArrow()
    {
        if (powerUp is SpecialArrowEffect)
        {
            SpecialArrowEffect temp = (SpecialArrowEffect)powerUp;
            float damageMultiplier = 1.0f;
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

            switch (chargeLevel)
            {
                case Enum_ChargeLevel.low:
                    damageMultiplier = baseChargeMultiplier;
                    break;
                case Enum_ChargeLevel.medium:
                    damageMultiplier = firstLevelMultiplier;
                    break;
                case Enum_ChargeLevel.high:
                    damageMultiplier = secondLevelMultiplier;
                    break;

            }
            ar.Damage = Mathf.CeilToInt(ar.Damage * damageMultiplier);

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
            ar.GetComponent<Rigidbody2D>().AddForce(new Vector2(forceArr * damageMultiplier, 0));
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
