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

    [SerializeField] protected int baseHealth = 100;
    [SerializeField] protected int determination = 3;
    [SerializeField] protected float speed = 10;

    [Header("Jump Settings")]
    [SerializeField] protected float jumpHeight = 10;
    [SerializeField] protected float fatiguedSpeedReduction = 1.2f;
    [SerializeField] protected float fallMultiplier;

    [Header("Dodge Settings")]
    [SerializeField] protected int dodgeStaminaCost = 30;
    [SerializeField] protected int dodgeFrames = 12;
    [SerializeField] protected int dodgeImmunityStartFrame = 2;
    [SerializeField] protected int dodgeImmunityEndFrame = 10;
    [SerializeField] protected int maxDodgeToken = 1;
    [SerializeField] protected int dodgeToken = 1; // limit the number of times you can dash in the air, you have to land to reset it;

    [Header("Parry Settings")]
    [SerializeField] protected int parryStaminaCost = 60;
    [SerializeField] protected int parryImmunityFrames = 30;

    [Header("Stamina Settings")]
    [SerializeField] protected float baseStamina = 100f;
    [SerializeField] protected float staminaRegenerationPerSecond = 15f;
    [SerializeField] protected float staminaRegenerationCooldown = 1.5f;
    [SerializeField] protected float primaryFireStaminaCost = 20f;
    [SerializeField] protected float secondaryFireStaminaCost = 40f;

    [Header("Limit Break Settings")]
    [SerializeField] protected float maxLimitBreakGauge = 100;
    [SerializeField] protected float limitBreakPerSecond = 0.40f;
    [SerializeField] protected float limitBreakOnHit = 2.5f;
    [SerializeField] protected float limitBreakOnDamage = 1.0f;
    
    protected int health;
    protected float stamina, staminablockedTimer, dodgeTimeStart, limitBreakGauge;
    protected float rightCol;
    protected int dodgeFrameCounter;
    protected float distToGround, facing, verticalDirection;
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
    }

    protected void LateUpdate()
    {
        RegenerateStaminaPerSecond();
        IncreaseLimitBreakPerSecond();
    }

    protected void DynamicFall()
    {
        if (rb != null && rb.velocity.y < 0 && !IsGrounded() && dodgeStatus == Enum_DodgeStatus.ready)
        {
            animator.SetBool("Jump", false);
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            animator.SetBool("Fall", true);
        }
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
                if (IsGrounded())
                {
                    dodgeToken = maxDodgeToken;
                }
                if (Input.GetButtonDown("Dodge") && inputStatus == Enum_InputStatus.allowed && !fatigued && dodgeToken > 0)
                {
                    dodgeFrameCounter = 0;
                    rb.velocity = new Vector2(0, 0);
                    rb.velocity += new Vector2(facing * 40, verticalDirection * 40);
                    ReduceStamina(dodgeStaminaCost);
                    dodgeStatus = Enum_DodgeStatus.dodging;
                    inputStatus = Enum_InputStatus.blocked;
                    dodgeToken--;
                    animator.SetBool("Jump", false);
                    animator.SetBool("Fall", false);
                    animator.SetBool("Dodge", true);
                }
                break;
            case Enum_DodgeStatus.dodging:
                dodgeFrameCounter++;
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
                    animator.SetBool("Dodge", false);
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

    public virtual void Jump()
    {
        if (rb != null && IsGrounded())
        {
            rb.AddForce(new Vector3(0, jumpHeight * rb.mass, 0), ForceMode2D.Impulse);
            animator.SetBool("Jump", true);
            jumping = false;
        }
    }

    public virtual bool IsGrounded()
    {
        //returns true if collides with an obstacle underneath object
        if (Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.1f, LayerMask.GetMask("Obstacle")))
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
            return true;
        }
        return false;
    }

    public void ReduceStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0);
        CheckFatigue();
        staminaRegenerationStatus = Enum_StaminaRegeneration.blocked;
    }

    public void ApplyDamage(int dmg)
    {
        if (health > dmg) health -= dmg;
        else
        {
            health = 0;
        }

    }

    public float Facing
    {
        get
        {
            return facing;
        }
    }
    public float Stamina
    {
        get
        {
            return stamina;
        }
    }
    public float BaseStamina
    {
        get
        {
            return baseStamina;
        }
    }
    public float DodgeStaminaCost
    {
        get
        {
            return dodgeStaminaCost;
        }
    }
    public Enum_InputStatus InputStatus
    {
        get
        {
            return inputStatus;
        }
    }
    public Enum_DodgeStatus Dodging
    {
        get
        {
            return dodgeStatus;
        }
    }
    public int BaseHealth
    {
        get
        {
            return baseHealth;
        }
    }
    public int Health
    { 
        get
        {
            return health;
        }
    }
    public bool Immunity
    {
        get
        {
            return immune;
        }
    }
    public bool Fatigue
    {
        get
        {
            return fatigued;
        }
    }
    
}
