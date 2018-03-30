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
    [SerializeField] protected float initialStaminaCost = 18.0f;
    [SerializeField] protected float maxStaminaConsumption = 10.0f;
    [SerializeField] protected float maxChargeTime = 5.0f;
    [SerializeField] protected ParticleSystem chargeParticleSystem;
    [SerializeField] protected Material[] chargeMaterials;

    [Header("ProjectileSettingArrow")]
    [SerializeField] protected GameObject arrow;
    [SerializeField] protected Vector2 projectileSpawnOffsetArrow;

    [Header("UltimateArrow")]
    [SerializeField] protected GameObject ultimateArrow;
    [SerializeField] protected Vector3[] arrowAngles = new Vector3[3];

    // Tenir bouton enfoncé
    protected float baseChargeMultiplier = 1.0f;
    protected float chargeTimer = 0.0f; //Our timer
    protected float forceArrow = 0.3f; //The force of the arrow
    protected bool isKeyActive = false;
    protected Enum_ChargeLevel chargeLevel = Enum_ChargeLevel.low;

    private List<UltimateArrow> arrowList = new List<UltimateArrow>();

    private float staminaConsumed;
    private bool playedOnceChargeOne = false, playedOnceChargeTwo = false;
    private ParticleSystemRenderer psr;

    protected override void Start()
    {
        base.Start();
        if(chargeParticleSystem != null)
        {
            DisableChargeParticleSystem();
            psr = chargeParticleSystem.GetComponent<ParticleSystemRenderer>();
        }
    }
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
        base.Update();
        
        if (Input.GetButton(SecondaryAttackButton) && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard && IsGrounded() && !dead)
        {
            if (Input.GetButtonDown(SecondaryAttackButton))
            {
                ReduceStamina(secondaryFireStaminaCost); //remove stamina once
            }
            Charge(); //only charge when grounded
        }
        else if (Input.GetButtonUp(SecondaryAttackButton) && !Fatigue)
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
        float staminaCost = secondaryFireStaminaCost * Time.deltaTime;
        if(chargeTimer == 0)
        {
            animator.SetTrigger("Draw");
            staminaConsumed = 0.0f;
            PlaySoundEffectRandomPitch("DrawArrowArcher");
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
            if(!playedOnceChargeTwo)
            {
                PlaySoundEffectRandomPitch("DrawArrowArcher");
                ChangeParticleMaterial(2);
            }
        }
        else if(chargeTimer >= firstLevelTime)
        {
            chargeLevel = Enum_ChargeLevel.medium;
            if (!playedOnceChargeOne)
            {
                ChangeParticleMaterial(1);
            }
        }
        else
        {
            chargeLevel = Enum_ChargeLevel.low;
        }

        if(staminaConsumed < maxStaminaConsumption)
        {
            ReduceStamina(secondaryFireStaminaCost * Time.deltaTime);
            staminaConsumed += staminaCost;
            
        }
        staminaRegenerationStatus = Enum_StaminaRegeneration.blocked;
        staminablockedTimer = 0.0f;
        if (Fatigue || chargeTimer >= maxChargeTime)
        {
            Release();
        }
    }

    private void Release()
    {
        //DisableChargeParticleSystem();
        animator.SetBool("Hold", false);
        StopSoundEffect("DrawArrowArcher");
        PlaySoundEffectRandomPitch("ArrowRelease");
        speed = baseSpeed;
        chargeTimer = 0.0f;
        playedOnceChargeTwo = false;
        playedOnceChargeOne = false;
    }

    private void DisableChargeParticleSystem()
    {
        chargeParticleSystem.Stop();
    }
    private void ChangeParticleMaterial(int level)
    {
        if(psr != null && chargeParticleSystem != null)
        {
            if(chargeMaterials[level - 1] != null)
            {
                psr.material = chargeMaterials[level - 1];
                chargeParticleSystem.Play();
            }
            if(level == 1)
            {
                playedOnceChargeOne = true;
            }
            else
            {
                playedOnceChargeTwo = true;
            }
        }   
    }
    public void SpawnArrow()
    {
        if (powerUp is SpecialArrowEffect)
        {
            SpecialArrowEffect temp = (SpecialArrowEffect)powerUp;
            float damageMultiplier = 1.0f;
            if (powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
            {
                temp.GetRandomPowerUp();
            }
            else
            {
                temp.ResetPowerUp();
            }

            Vector2 SpawnPoint = new Vector2(transform.position.x + projectileSpawnOffsetArrow.x * facing, transform.position.y + projectileSpawnOffsetArrow.y);
            GameObject arrowProjectile = Instantiate(arrow, SpawnPoint, transform.rotation);
            Arrow ar = arrowProjectile.GetComponent<Arrow>();
            ar.Owner = this;
            ar.Direction = facing;
            
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
            if(powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
            {
                ar.ArrowStatus = temp.StatusEffect;
                temp.UseStack();
            }
            else
            {
                ar.ArrowStatus = Enum_SpecialStatus.normal;
            }
            
            Vector2 forceArr = ar.Force * facing;
            ar.GetComponent<Rigidbody2D>().AddForce(forceArr * damageMultiplier, 0);
            rb.gravityScale = 1.0f;
            AllowInputs();
        }
    }

    protected override void CastHitBox(int attackType)
    {
        SpecialArrowEffect temp = (SpecialArrowEffect)powerUp;
        if (powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
        {
            temp.GetRandomPowerUp();
            combo1.haveSpecialEffect = true;
            combo1.specialEffect = temp.StatusEffect; //appliquer l'effet sur l'attaque
        }
        else
        {
            temp.ResetPowerUp();
            combo1.haveSpecialEffect = false;
            combo1.specialEffect = Enum_SpecialStatus.normal;
        }


        switch (attackType)
        {
            case 1:
                combo1.CastHitBox();
                if (powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
                {
                    temp.UseStack();
                }
                break;
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }

    protected override void Ultimate()
    {
        EndAttackString();
        //inputStatus = Enum_InputStatus.blocked;
        animator.SetBool("Ultimate", true);
        rb.velocity = Vector2.zero;
        ClearArrowList();
        //immune = true;
        Vector2 SpawnPoint = new Vector2(transform.position.x + projectileSpawnOffsetArrow.x * facing, transform.position.y + projectileSpawnOffsetArrow.y);
        for(int i = 0; i < arrowAngles.Length; i++)
        {
            GameObject arrowProjectile = Instantiate(ultimateArrow, SpawnPoint, transform.rotation);
            
            UltimateArrow ar = arrowProjectile.GetComponent<UltimateArrow>();
            arrowList.Add(ar);
            ar.Owner = this;
            ar.Direction = facing;
            Physics2D.IgnoreCollision(ar.GetComponent<Collider2D>(), physicBox);
            float xForce = ar.Force.magnitude * Mathf.Abs(Mathf.Cos(arrowAngles[i].z)) * facing;
            float yForce = ar.Force.magnitude * Mathf.Abs(Mathf.Sin(arrowAngles[i].z)) * Mathf.Sign(arrowAngles[i].z);
            Debug.Log(xForce +" "+ yForce);
            switch (i)
            {
                case 0:
                    ar.ArrowStatus = Enum_SpecialStatus.stun;
                    break;
                case 1:
                    ar.ArrowStatus = Enum_SpecialStatus.poison;
                    break;
                case 2:
                    ar.ArrowStatus = Enum_SpecialStatus.slow;
                    break;
                default:
                    ar.ArrowStatus = Enum_SpecialStatus.normal;
                    break;
            }
            Vector2 forceArr = new Vector2(xForce, yForce);
            ar.GetComponent<Rigidbody2D>().AddForce(forceArr);
        }
        
        ResetLimitBreak();
    }

    public override void ApplyStunLock(int duration)
    {
        base.ApplyStunLock(duration);
        chargeTimer = 0.0f;
        animator.SetBool("Hold", false);
    }

    public void ClearArrowList()
    {
        foreach(UltimateArrow ua in arrowList)
        {
            if(ua != null && ua.gameObject != null)
            {
                Destroy(ua.gameObject);
            }
        }
        arrowList.Clear();
    }
}
