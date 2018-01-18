using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enum_InputStatus
{
    blocked,
    allowed
}

public enum Enum_DodgeStatus
{
    ready,
    dodging
}

public enum Enum_StaminaRegeneration
{
    regenerating,
    blocked
}

public abstract class Champion : MonoBehaviour {

    [SerializeField]
    protected int baseHealth = 100, 
        determination = 3, 
        dodgeStaminaCost = 30, 
        parryStaminaCost = 60, 
        dodgeFrames = 12,
        dodgeImmunityStartFrame = 2,
        dodgeImmunityEndFrame = 10,
        parryImmunityFrames = 30;
    [SerializeField]
    protected float baseStamina = 100f,
        staminaRegenerationPerSecond = 15f,
        staminaRegenerationCooldown = 1.5f,

        maxLimitBreakGauge = 100,
        limitBreakPerSecond = 0.40f,
        limitBreakOnHit = 2.5f,
        limitBreakOnDamage = 1.0f,

        primaryFireStaminaCost = 20f,
        secondaryFireStaminaCost = 40f,

        speed = 10,
        jumpHeight = 10,
        fatiguedSpeedReduction = 1.2f;
        
    [SerializeField]
    protected float fallMultiplier;
    
    protected int health;
    protected float stamina, staminablockedTimer, dodgeTimeStart, limitBreakGauge;
    protected float rightCol;
    protected int dodgeFrameCounter;
    protected float distToGround, facing, verticalDirection;
    protected bool flying;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 savedVelocity;
    protected bool jumping, immune = false, parrying = false, fatigued = false;
    protected Enum_InputStatus inputStatus = Enum_InputStatus.allowed;
    protected Enum_DodgeStatus dodgeStatus = Enum_DodgeStatus.ready;
    protected Enum_StaminaRegeneration staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;

    protected void Start()
    {
        health = baseHealth;
        stamina = baseStamina;
        limitBreakGauge = 0.0f;
        distToGround = GetComponent<Collider2D>().bounds.extents.y;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    protected void FixedUpdate()
    {
        DynamicFall();
        if (jumping)
        {
            Jump();
        }
    }

    protected void Update()
    {
        CheckFatigue();
        CheckDodge();
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumping = true;
        }
        Debug.Log(rb.gravityScale);
    }

    protected void LateUpdate()
    {
        RegenerateStaminaPerSecond();
        IncreaseLimitBreakPerSecond();
    }
    protected void DynamicFall()
    {
        if (rb != null && rb.velocity.y < 0 && !IsGrounded() && !flying && dodgeStatus == Enum_DodgeStatus.ready)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        if (IsGrounded())
        {
            flying = false;
            rb.gravityScale = 1;
        }
        Debug.Log(IsGrounded() + ", "+flying);
    }

    protected virtual void RegenerateStaminaPerSecond()
    {
        float staminaRegen = staminaRegenerationPerSecond;
        switch (staminaRegenerationStatus)
        {
            case Enum_StaminaRegeneration.regenerating:
                stamina = Mathf.Min(stamina + staminaRegen * Time.deltaTime, baseStamina);
                staminablockedTimer = 0.0f;
                break;
            case Enum_StaminaRegeneration.blocked:
                staminablockedTimer += Time.deltaTime;
                if(staminablockedTimer > staminaRegenerationCooldown)
                {
                    staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
                }
                break;
        }
    }

    protected virtual void IncreaseLimitBreakPerSecond()
    {
        limitBreakGauge = Mathf.Min(limitBreakGauge + limitBreakPerSecond * Time.deltaTime, maxLimitBreakGauge);
    }
    protected abstract void PrimaryAttack();
    protected abstract void SecondaryAttack();

    protected void CheckFatigue()
    {
        if(stamina == 0)
        {
            fatigued = true;
        }
        else if(stamina == baseStamina)
        {
            fatigued = false;
        }
    }
    protected void CheckDodge()
    {
        switch (dodgeStatus)
        {
            case Enum_DodgeStatus.ready:
                if (Input.GetButtonDown("Dodge") && inputStatus == Enum_InputStatus.allowed && !fatigued)
                {
                    dodgeFrameCounter = 0;
                    rb.velocity = new Vector2(0, 0);
                    rb.velocity += new Vector2(facing * 40, verticalDirection * 40);
                    ReduceStamina(dodgeStaminaCost);
                    dodgeStatus = Enum_DodgeStatus.dodging;
                    inputStatus = Enum_InputStatus.blocked;
                }
                break;
            case Enum_DodgeStatus.dodging:
                rb.gravityScale = 0.0f;
                dodgeFrameCounter +=3;
                if(dodgeFrameCounter >= dodgeImmunityStartFrame && dodgeFrameCounter < dodgeImmunityEndFrame)
                {
                    immune = true;
                }
                if(dodgeFrameCounter >= dodgeImmunityEndFrame)
                {
                    immune = false;
                }
                if (dodgeFrameCounter >= dodgeFrames)
                {
                    dodgeFrameCounter = dodgeFrames;
                    rb.velocity = new Vector2(0, 0);
                    dodgeStatus = Enum_DodgeStatus.ready;
                    inputStatus = Enum_InputStatus.allowed;
                }
                break;
        }
    }
    
    public virtual void Move(float moveX, float moveY)
    {
        float currentSpeed = fatigued ? speed / fatiguedSpeedReduction : speed;
        //LIMIT DIAGONAL SPEED
        Vector2 movement = new Vector2(moveX, moveY).normalized * currentSpeed;

        //not impeding X movements when aerial
        
        transform.Translate(movement * Time.deltaTime);
        if(moveX != 0 || moveY !=0)
        {
            animator.SetBool("Moving", true);
            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);
            animator.SetFloat("FaceX", moveX);
            facing = moveX;
            verticalDirection = moveY;
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    public float GetFacing()
    {
        return facing;
    }
    public virtual void Jump()
    {
        if (rb != null && IsGrounded())
        {
            rb.AddForce(new Vector3(0, jumpHeight * rb.mass, 0), ForceMode2D.Impulse);
            jumping = false;
        }
    }

    public virtual bool IsGrounded()
    {
        //returns true if collides with an obstacle underneath object

        if (Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.1f, LayerMask.GetMask("Obstacle")))
        {
            return true;
        }
        return false;
    }

    public virtual void SetFlying()
    {
        flying = !flying;
        if (flying)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0.0f;
            
        }
        else
        {
            rb.gravityScale = 1.0f;
        }
    }

    public bool GetFlying()
    {
        return flying;
    }

    public Enum_InputStatus GetInputStatus()
    {
        return inputStatus;
    }

    public int GetBaseHealth()
    {
        return baseHealth;
    }
    public int GetHealth()
    {
        return health;
    }

    public void ReduceStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0);
        CheckFatigue();
        staminaRegenerationStatus = Enum_StaminaRegeneration.blocked;
    }
    public float GetStamina()
    {
        return stamina;
    }
    public float GetBaseStamina()
    {
        return baseStamina;
    }
    public float GetDodgeStaminaCost()
    {
        return dodgeStaminaCost;
    }
    public Enum_DodgeStatus Dodging()
    {
        return dodgeStatus;
    }

    public bool GetImmunity()
    {
        return immune;
    }
    public bool GetFatigue()
    {
        return fatigued;
    }
    
}
