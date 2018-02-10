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

public enum Enum_GuardStatus
{
    noGuard,
    guarding,
    parrying
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
    [SerializeField] protected float jumpVelocityAtApex = 2.0f;

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

    [Header("Guard Settings")]
    [SerializeField] protected float damageReductionMultiplier = 0.2f;
    [SerializeField] protected float blockStaminaCostMultiplier = 2.0f;

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
    [SerializeField] protected int comboOneStunLock = 5;
    [SerializeField] protected Vector2 comboOneRecoilForce;
    
    [Header("HUDSettings")]
    [SerializeField] protected CanvasGroup playerHUD;

    protected int health, framesToStunLock = 0, stunlockFrameCounter = 0;
    protected float stamina, staminablockedTimer, dodgeTimeStart, limitBreakGauge;
    protected int dodgeFrameCounter;
    protected float distToGround, facing;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 savedVelocity;
    protected Collider2D playerBox;
    protected Collider2D physicBox;
    protected bool jumping, immune = false, parrying = false, fatigued = false, attacking = false, dead = false;
    protected Enum_InputStatus inputStatus = Enum_InputStatus.allowed;
    protected Enum_DodgeStatus dodgeStatus = Enum_DodgeStatus.ready;
    protected Enum_StaminaRegeneration staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
    protected Enum_GuardStatus guardStatus = Enum_GuardStatus.noGuard;

    protected Slider healthSlider;
    protected Slider staminaSlider;

    // INPUTS valeurs par défaut
    protected string HorizontalCtrl = "Horizontal";
    protected string VerticalCtrl = "Vertical";
    protected string JumpButton = "Jump";
    protected string DodgeButton = "Dodge";
    protected string PrimaryAttackButton = "PrimaryAttack";
    protected string PowerUpButton = "Up";
    protected string GuardButton = "Guard";


    protected float movementX, movementY;

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
        physicBox = GetComponent<Collider2D>();
        distToGround = physicBox.bounds.extents.y;
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
            CheckStunLock();
            CheckFatigue();
            CheckDodge();
            if (InputStatus == Enum_InputStatus.blocked)
            {
                StopMovement(0);
            }
            else
            {
                if (Input.GetAxisRaw(PowerUpButton) != 0 && powerUp != null && powerUp.PowerUpStatus == Enum_PowerUpStatus.available)
                {
                    Debug.Log("PowerUp");
                    powerUp.ActivatePowerUp();
                }

                if (IsGrounded() && InputStatus != Enum_InputStatus.onlyMovement)
                {
                    if (Input.GetButtonDown(PrimaryAttackButton) && guardStatus == Enum_GuardStatus.noGuard)
                    {
                        PrimaryAttack();
                    }

                    if(Input.GetAxisRaw(GuardButton) != 0 && guardStatus != Enum_GuardStatus.parrying)
                    {
                        guardStatus = Enum_GuardStatus.guarding;
                        animator.SetBool("Guarding", true);
                    }
                    if(Input.GetAxis(GuardButton) != 1)
                    {
                        guardStatus = Enum_GuardStatus.noGuard;
                        animator.SetBool("Guarding", false);
                    }
                }

                if (InputStatus != Enum_InputStatus.onlyAttack)
                {
                    if (Input.GetButtonDown(JumpButton) && IsGrounded())
                    {
                        jumping = true;
                    } else
                    {
                        if (Input.GetButtonDown(JumpButton) && !IsGrounded())
                        {
                            jumping = true;
                        }
                    }

                    if (InputStatus != Enum_InputStatus.blocked)
                    {
                        movementX = Input.GetAxisRaw(HorizontalCtrl);
                        if (!IsGrounded() && Input.GetAxis(VerticalCtrl) == -1 )
                        {
                            Fall();
                        }
                    }

                }
                
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (!dead)
        {
            RegenerateStaminaPerSecond();
            IncreaseLimitBreakPerSecond();
        }
        UpdateHUD();
    }

    protected void DynamicFall()
    {
        if (rb != null && rb.velocity.y < jumpVelocityAtApex && !IsGrounded() && dodgeStatus == Enum_DodgeStatus.ready && attacking == false)
        {
            Fall();
        }
    }

    protected void Fall()
    {
        animator.SetBool("Jump", false);
        rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

            animator.SetBool("Fall", true);
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
                    guardStatus = Enum_GuardStatus.noGuard;
                    animator.SetBool("Guarding", false);
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

    public void ApplyStunLock(int duration) // Player can't execute action while damaged
    {
        stunlockFrameCounter = 0;
        framesToStunLock = duration;
        inputStatus = Enum_InputStatus.blocked;
        animator.SetBool("Damaged", true);
    }

    public void CheckStunLock()
    {
        if(framesToStunLock > 0)
        {
            stunlockFrameCounter++;
            if (stunlockFrameCounter >= framesToStunLock)
            {
                framesToStunLock = 0;
                stunlockFrameCounter = 0;
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetBool("Damaged", false);
                inputStatus = Enum_InputStatus.allowed;
            }
        }
    }

    public void ApplyDamage(int dmg, float attackerFacing, int stunLock, Vector2 recoilForce)
    {
        if (!Immunity )
        {
            if(guardStatus == Enum_GuardStatus.guarding && attackerFacing != facing) // attacker is in front of the player and player is guarding
            {
                health = Mathf.Max(health - (int)Mathf.Ceil(dmg * damageReductionMultiplier), 0);
                ReduceStamina(dmg * blockStaminaCostMultiplier);
                animator.SetTrigger("Blocked");
            }
            else //attacker is behind the player or player is not guarding
            {
                health = Mathf.Max(health - dmg, 0);
                animator.SetFloat("AttackerFacing", attackerFacing);
                ApplyStunLock(stunLock);
                rb.AddForce(recoilForce * attackerFacing, ForceMode2D.Impulse);
            }
        }

        Debug.Log("Health :" + health);

        //TO REMOVE LATER
        if (health == 0)
        {
            inputStatus = Enum_InputStatus.blocked;
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                    animator.SetBool(parameter.name, false);
            }
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
                if (Input.GetButtonDown(DodgeButton) && inputStatus == Enum_InputStatus.allowed && 
                    guardStatus == Enum_GuardStatus.noGuard && !fatigued && dodgeToken > 0)
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
        if (moveX != 0)
        {
            facing = Mathf.Sign(moveX);
            animator.SetFloat("FaceX", facing);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
        if (guardStatus == Enum_GuardStatus.noGuard && moveX != 0)
        {

            animator.SetBool("Moving", true);
            transform.Translate(movement * Time.deltaTime);
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
        } else
        {
            if (rb != null && !IsGrounded())
            {
                rb.AddForce(new Vector2(0, - jumpHeight * rb.mass), ForceMode2D.Impulse);
                animator.SetBool("Jump", true);
                jumping = false;
            }
        }
    }

    public virtual bool IsGrounded()
    {
        //returns true if collides with an obstacle underneath object
        Vector2 center = new Vector2(physicBox.bounds.center.x, physicBox.bounds.min.y);//+ (Vector2)transform.position;
        float radius = 0.2f;

        if(Physics2D.OverlapCircle(center, radius, LayerMask.GetMask("Obstacle")))
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
            return true;
        }
        return false;
    }

    public void SetHorizontalCtrl(string HCtrl)
    {
        HorizontalCtrl = HCtrl;
    }

    public void SetVerticalCtrl(string VCtrl)
    {
        VerticalCtrl = VCtrl;
    }

    public void SetJumpButton(string JButton)
    {
        JumpButton = JButton;
    }

    public void SetDodgeButton(string DButton)
    {
        DodgeButton = DButton;
    }

    public void SetPrimaryAttackButton(string PAButton)
    {
        PrimaryAttackButton = PAButton;
    }

    public void SetPowerUpButton(string PUButton)
    {
        PowerUpButton = PUButton;
    }

    public void SetGuardButton(string GButton)
    {
        GuardButton = GButton;
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

    public bool Dead
    {
        get
        {
            return dead;
        }
    }
}
