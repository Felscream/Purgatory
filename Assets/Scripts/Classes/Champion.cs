using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public enum Enum_InputStatus
{
    blocked,
    allowed,
    onlyAttack,
    onlyMovement
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
    [SerializeField] protected float dodgeSpeed = 40.0f;
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
    [SerializeField] public float baseStamina = 100f;
    [SerializeField] protected float staminaRegenerationPerSecond = 15f;
    [SerializeField] protected float staminaRegenerationCooldown = 1.5f;
    [SerializeField] protected float staminaFatigueCooldown = 3.0f;
    [SerializeField] protected float primaryFireStaminaCost = 20f;
    [SerializeField] protected float secondaryFireStaminaCost = 40f;

    [Header("Limit Break Settings")]
    [SerializeField] protected float maxLimitBreakGauge = 100;
    [SerializeField] protected float limitBreakPerSecond = 0.40f;
    [SerializeField] protected float limitBreakOnHit = 2.5f;
    [SerializeField] protected float limitBreakOnDamage = 1.0f;

    [Header("Attack Settings")]
    [SerializeField] protected int comboOneDamage = 10;
    [SerializeField] protected float primaryAttackMovementForce = 2;
    [SerializeField] protected LayerMask hitBoxLayer;

    [Header("Combo1Settings")]
    [SerializeField] protected float comboOneSizeX = 1;
    [SerializeField] protected float comboOneSizeY = 1;
    [SerializeField] protected float comboOneOffsetX = 0;
    [SerializeField] protected float comboOneOffsetY = 0;

    [Header("HUDSettings")]
    [SerializeField] protected CanvasGroup playerHUD;

    protected int health;
    protected float stamina, staminablockedTimer, dodgeTimeStart, limitBreakGauge;
    protected int dodgeFrameCounter;
    protected float distToGround, facing;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 savedVelocity;
    protected Collider2D playerBox;
    protected bool jumping, immune = false, parrying = false, fatigued = false, attacking = false, dead = false;
    protected Enum_InputStatus inputStatus = Enum_InputStatus.allowed;
    protected Enum_DodgeStatus dodgeStatus = Enum_DodgeStatus.ready;
    protected Enum_StaminaRegeneration staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;

    protected Slider healthSlider;
    protected Slider staminaSlider;

    // valeurs par défaut
    private string HorizontalCtrl = "Horizontal";
    private string JumpButton = "Jump";
    private string DodgeButton = "Dodge";
    private string PrimaryAttackButton = "PrimaryAttack";
    private string PowerUpButton = "PowerUp";
    private float movementX, movementY;

    protected PowerUp powerUp;

    protected void Awake()
    {
        facing = (transform.parent.gameObject.name == "Player1" || transform.parent.gameObject.name == "Player3") ? 1.0f : -1.0f;
    }
    protected void Start()
    {   
        health = baseHealth;
        stamina = baseStamina;
        limitBreakGauge = 0.0f;
        distToGround = GetComponent<Collider2D>().bounds.extents.y;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("FaceX", facing);
        playerBox = transform.Find("PlayerBox").GetComponentInChildren<Collider2D>();
        powerUp = GetComponent<PowerUp>();

        playerHUD.alpha = 1;
        healthSlider = playerHUD.transform.Find("HealthSlider").GetComponent<Slider>();
        staminaSlider = playerHUD.transform.Find("StaminaSlider").GetComponent<Slider>();
        UpdateHUD();
    }

    protected void FixedUpdate()
    {
        DynamicFall();
        if (jumping)
        {
            Jump();
        }
        Move(movementX, movementY);
    }

    protected virtual void Update()
    {
        if (!dead)
        {
            CheckFatigue();
            CheckDodge();
            UpdateHUD();
            if (InputStatus == Enum_InputStatus.blocked)
            {
                StopMovement(0);
            }
            else
            {

                if (IsGrounded() && InputStatus != Enum_InputStatus.onlyMovement)
                {
                    if (Input.GetButtonDown(PrimaryAttackButton))
                    {
                        PrimaryAttack();
                    }
                    if(Input.GetAxisRaw(PowerUpButton) != 0 && powerUp!= null && powerUp.PowerUpStatus == Enum_PowerUpStatus.available)
                    {
                        powerUp.ActivatePowerUp();
                    }
                    /*Debug.Log(animator.GetCurrentAnimatorStateInfo(0).tagHash);
                    Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Right_Combo_1_Normal_Attack_Knight"));*/
                }

                if (InputStatus != Enum_InputStatus.onlyAttack)
                {
                    if (Input.GetButtonDown(JumpButton) && IsGrounded())
                    {
                        jumping = true;
                    }

                    if (InputStatus != Enum_InputStatus.blocked)
                    {
                        movementX = Input.GetAxisRaw(HorizontalCtrl);
                        //MOVING

                    }

                }
                
            }
        }
    }

    protected virtual void LateUpdate()
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
                break;
            case Enum_StaminaRegeneration.blocked:
                staminablockedTimer += Time.deltaTime;
                if (Fatigue){
                    inputStatus = Enum_InputStatus.onlyMovement;
                    if (staminablockedTimer > staminaFatigueCooldown && !attacking)
                    {
                        staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
                        inputStatus = Enum_InputStatus.allowed;
                    }
	            }
                else{
                    if(staminablockedTimer > staminaRegenerationCooldown)
                    {
                        staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;

                    }
	            }
                break;
        }
    }

    protected virtual void IncreaseLimitBreakPerSecond()
    {
        limitBreakGauge = Mathf.Min(limitBreakGauge + limitBreakPerSecond * Time.deltaTime, maxLimitBreakGauge);
    }

    protected virtual void PrimaryAttack()
    {
        animator.SetTrigger("PrimaryAttack");
    }

    protected virtual void SecondaryAttack()
    {

    }
    
    protected abstract void CastHitBox(int attackType);
    protected virtual void MoveOnAttack()
    {
        Vector2 force = new Vector2(facing * primaryAttackMovementForce, 0);
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    protected void StartAttackString()
    {
        ReduceStamina(primaryFireStaminaCost);
        inputStatus = Enum_InputStatus.onlyAttack;
        movementX = 0;
        movementY = 0;
        attacking = true;
        rb.velocity = Vector2.zero;
    }
    protected virtual void EndAttackString()
    {
        inputStatus = Enum_InputStatus.allowed;
        CheckFatigue();
        attacking = false;
    }
    public void ReduceStamina(float amount)
    {
        if (amount != 0.0f)
        {
            stamina = Mathf.Max(stamina - amount, 0);
            CheckFatigue();
            staminaRegenerationStatus = Enum_StaminaRegeneration.blocked;
            staminablockedTimer = 0.0f;
        }
    }

    public void ApplyDamage(int dmg)
    {
        /*if (health > dmg)
        {
            health -= dmg;
        }
        else
        {
            health = 0;
        }*/
        if (!Immunity)
        {
            health = Mathf.Max(health - dmg, 0);
        }

        Debug.Log("Health :" + health);

        //TO REMOVE LATER
        if (health == 0)
        {
            inputStatus = Enum_InputStatus.blocked;
            dead = true;
            playerBox.enabled = false;
            Debug.Log(transform.parent.name + " died");
        }
    }
    protected void CheckFatigue()
    {
        if(stamina == 0)
        {
            fatigued = true;
        }
        else //if(stamina >= baseStamina*0.25f) // Au moins 25% de la stamina
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
                if (Input.GetButtonDown(DodgeButton) && inputStatus == Enum_InputStatus.allowed && !fatigued && dodgeToken > 0)
                {
                    dodgeFrameCounter = 0;
                    playerBox.enabled = false;
                    rb.velocity = new Vector2(0, 0);
                    rb.AddForce(new Vector2(facing * dodgeSpeed,0), ForceMode2D.Impulse);
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
                    playerBox.enabled = true;
                    dodgeStatus = Enum_DodgeStatus.ready;
                    inputStatus = Enum_InputStatus.allowed;
                    
                }
                break;
        }
    }
    
    public virtual void Move(float moveX, float moveY)
    {
        float currentSpeed = Fatigue ? speed / fatiguedSpeedReduction : speed;
        //LIMIT DIAGONAL SPEED
        Vector2 movement = new Vector2(moveX, moveY).normalized * currentSpeed;

        //not impeding X movements when aerial
        
        transform.Translate(movement * Time.deltaTime);
        if(moveX != 0)
        {
            animator.SetBool("Moving", true);
            animator.SetFloat("MoveX", Mathf.Sign(moveX));
            facing = Mathf.Sign(moveX);
            animator.SetFloat("FaceX", facing);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    protected void StopMovement(int stopForce)
    {
        movementX = 0;
        movementY = 0;
        if (stopForce == 1)
        {
            rb.velocity = Vector2.zero;
        }
    }
    public virtual void Jump()
    {
        if (rb != null && IsGrounded())
        {
            rb.AddForce(new Vector2(0, jumpHeight * rb.mass), ForceMode2D.Impulse);
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

    public virtual void SetHorizontalCtrl(string HCtrl)
    {
        HorizontalCtrl = HCtrl;
    }

    public virtual void SetJumpButton(string JButton)
    {
        JumpButton = JButton;
    }

    public virtual void SetDodgeButton(string DButton)
    {
        DodgeButton = DButton;
    }

    public virtual void SetPrimaryAttackButton(string PAButton)
    {
        PrimaryAttackButton = PAButton;
    }

    public virtual void SetPowerUpButton(string PUButton)
    {
        PowerUpButton = PUButton;
    }

    public void UpdateHUD()
    {
        healthSlider.value = health;
        staminaSlider.value = stamina;
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
    
    public bool Attack
    {
        get
        {
            return attacking;
        }
    }
}
