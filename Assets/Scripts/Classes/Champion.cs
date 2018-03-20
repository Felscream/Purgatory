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
public enum Enum_SpecialStatus
{
    normal,
    stun,
    poison,
    slow,
    projected
}

public abstract class Champion : MonoBehaviour {

    [SerializeField] protected float baseHealth = 100;
    [SerializeField] public int determination = 3;
    [SerializeField] protected float baseSpeed = 10;
    [SerializeField] protected LayerMask deadLayer;
    [Header("HUDSettings")]
    [SerializeField] public CanvasGroup playerHUD;

    [Header("Jump Settings")]
    [SerializeField] protected float jumpHeight = 10;
    [SerializeField] protected float fallMultiplier;
    [SerializeField] protected float jumpVelocityAtApex = 2.0f;
    [SerializeField] protected int coyoteTimeFrames = 6; //To implement coyote time

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
    [SerializeField] protected int parryImmunityStartFrame = 2;
    [SerializeField] protected int parryImmunityEndFrame = 8;
    [SerializeField] protected float parryStunDuration = 2.0f;

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
    [SerializeField] protected float limitBreakOnParry = 15.0f;

    [Header("Status Settings")]
    [SerializeField] protected float fatiguedSpeedReduction = 1 / 1.2f;
    [SerializeField] protected float slowSpeedReduction = 1 / 1.2f;

    [Header("Guard Break Settings")]
    [SerializeField] protected int guardBreakDamage = 5;
    [SerializeField] protected int guardBreakstunLock = 15;
    [SerializeField] protected Vector2 guardBreakRecoilForce;

    [Header("UltimateSettings")]
    [SerializeField] protected ParticleSystem ultimateParticleSystem;
    [SerializeField] protected float zoomWaitDuration = 1.0f;

    [Header("PowerUpSettings")]
    [SerializeField]
    protected ParticleSystem powerUpParticleSystem;

    [Header("Attack Settings")]
    [SerializeField] protected LayerMask hitBoxLayer;
    [SerializeField] protected int maxAttackToken = 1;

    public Attack specialAttack;
    public Attack combo1;
    
    protected int framesToStunLock = 0, stunlockFrameCounter = 0;
    protected float health, speed;
    protected float stamina, staminablockedTimer, dodgeTimeStart, limitBreakGauge;
    protected int dodgeFrameCounter = 0;
    protected int coyoteFrameCounter = 0;
    protected int parryFrameCounter = 0;
    protected int attackToken = 1;
    protected float distToGround, facing;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 savedVelocity;
    protected Vector2 wallColliderPosition;
    protected Collider2D playerBox;
    protected Collider2D physicBox;
    protected Collider2D diveBox;
    protected bool jumping, falling = false, immune = false, parrying = false, fatigued = false, attacking = false, dead = false, isClashing=false;
    protected Enum_InputStatus inputStatus = Enum_InputStatus.allowed;
    protected Enum_DodgeStatus dodgeStatus = Enum_DodgeStatus.ready;
    protected Enum_StaminaRegeneration staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
    protected Enum_GuardStatus guardStatus = Enum_GuardStatus.noGuard;
    protected Enum_SpecialStatus specialStatus = Enum_SpecialStatus.normal;
    protected float movementX, movementY;
    protected PowerUp powerUp;
    protected bool ignorePlatforms = false;
    protected Lever trapLever;
    protected Coroutine projectedCoroutine;
    protected const int  DEFAULT_EFFECT_DURATION = 3;

    [System.NonSerialized] public int clashClick=0;

    protected Slider healthSlider;
    protected Slider staminaSlider;
    protected Slider limitBreakSlider;
    protected int timerDamageHUD = 40;
    protected Image ultiImageSlider;

    protected SpriteRenderer sr;
    protected CameraControl cameraController;
    // INPUTS valeurs par défaut
    protected string HorizontalCtrl = "Horizontal";
    protected string VerticalCtrl = "Vertical";
    protected string JumpButton = "Jump";
    protected string DodgeButton = "Dodge";
    protected string PrimaryAttackButton = "PrimaryAttack";
    protected string SecondaryAttackButton = "SecondaryAttack";
    protected string PowerUpButton = "Up";
    protected string GuardButton = "Guard";
    protected string ParryButton = "Parry";
    protected string ActionButton = "Action";

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(new Vector3(physicBox.bounds.center.x - (physicBox.bounds.extents.x/2) * facing, physicBox.bounds.min.y, 0), 0.2f); //to visualize the ground detector
        //Gizmos.DrawSphere(new Vector3(physicBox.bounds.center.x + (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y,0), 0.2f);
        //Gizmos.DrawWireSphere(new Vector3(physicBox.bounds.center.x + physicBox.bounds.extents.x * -facing, physicBox.bounds.center.y, 0),0.5f);
    }
    protected void Awake()
    {
        facing = (transform.parent.gameObject.name == "Player1" || transform.parent.gameObject.name == "Player3") ? 1.0f : -1.0f;
        coyoteFrameCounter = coyoteTimeFrames; //to allow players to fall at the beginning
    }
    protected virtual void Start()
    {
        health = baseHealth;
        stamina = baseStamina;
        speed = baseSpeed;
        limitBreakGauge = 0.0f;
        physicBox = GetComponent<Collider2D>();
        
        distToGround = physicBox.bounds.extents.y;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("FaceX", facing);
        playerBox = transform.Find("PlayerBox").GetComponentInChildren<Collider2D>();
        diveBox = transform.Find("DiveBox").GetComponentInChildren<Collider2D>();
        powerUp = GetComponent<PowerUp>();
        
        combo1.SetUser(this);
        combo1.Clasheable();
        specialAttack.SetUser(this);

        playerHUD.alpha = 1;
        healthSlider = playerHUD.transform.Find("HealthSlider").GetComponent<Slider>();
        staminaSlider = playerHUD.transform.Find("StaminaSlider").GetComponent<Slider>();
        limitBreakSlider = playerHUD.transform.Find("LimitBreakSlider").GetComponent<Slider>();
        ultiImageSlider = playerHUD.transform.Find("UltiImage").Find("RadialSliderImage").GetComponent<Image>();
        UpdateHUD();
        ResetAttackTokens();

        cameraController = Camera.main.GetComponent<CameraControl>();
        sr = GetComponent<SpriteRenderer>();
        if(powerUpParticleSystem != null)
        {
            ParticleSystem.EmissionModule temp = powerUpParticleSystem.emission;
            temp.enabled = false;
        }
        if (ultimateParticleSystem != null)
        {
            ParticleSystem.EmissionModule temp = ultimateParticleSystem.emission;
            temp.enabled = false;
        }
    }
    protected void FixedUpdate()
    {
        if(specialStatus != Enum_SpecialStatus.projected || dead)
        {
            DynamicFall();
        }
        if (jumping && guardStatus == Enum_GuardStatus.noGuard)
        {
            Jump();
            jumping = false;
        }
        Move(movementX, movementY);
        if (dead)
        {
            
            if(!IsGrounded())
            {
                Debug.Log(IsGrounded());
                Fall();
            }
        }
    }
    protected virtual void Update()
    {
        if (!dead)
        {
            
            RegenerateStaminaPerSecond();
            RegenerateLimitBreakPerSecond();
            ControlCoyote();
            CheckStunLock();
            CheckFatigue();
            if (isClashing)
            {
                if (Input.GetButtonDown(JumpButton))
                {
                    clashClick++;
                }
                return;
            }
            CheckDodge();
            CheckParry();

            if (InputStatus == Enum_InputStatus.blocked)
            {
                StopMovement(0);
            }
            else
            {
                if(Input.GetAxis(PowerUpButton) >= 0.6f && Input.GetAxis(GuardButton) >= 0.6f)
                {
                    CheckUltimate();
                }
                else if (Input.GetAxis(PowerUpButton) >= 0.6f && powerUp != null && powerUp.PowerUpStatus == Enum_PowerUpStatus.available)
                {
                    if(powerUp != null)
                    {
                        powerUp.ActivatePowerUp();
                    }
                }
                
                if (Input.GetButtonDown(SecondaryAttackButton) && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard)
                {
                    SecondaryAttack();
                }

                // Action button
                if (Input.GetButtonDown(ActionButton) && trapLever != null && InputStatus != Enum_InputStatus.onlyMovement)
                {
                    Debug.Log("ActionButton");
                    if ( trapLever.canEngage)
                    {
                        trapLever.Engage();
                    }
                }

                if (IsGrounded() && InputStatus != Enum_InputStatus.onlyMovement)
                {
                    if (guardStatus == Enum_GuardStatus.noGuard && !Fatigue)
                    {
                        if (Input.GetButtonDown(PrimaryAttackButton))
                        {
                            PrimaryAttack();
                        }
                    }

                    if (Input.GetAxisRaw(GuardButton) != 0 && guardStatus != Enum_GuardStatus.parrying && !Fatigue)
                    {
                        guardStatus = Enum_GuardStatus.guarding;
                        animator.SetBool("Guarding", true);
                    }
                    if (Input.GetAxis(GuardButton) != 1)
                    {
                        guardStatus = Enum_GuardStatus.noGuard;
                        animator.SetBool("Guarding", false);
                    }
                }

                if (InputStatus != Enum_InputStatus.onlyAttack )
                {
                    if (IsGrounded() && Input.GetAxis(VerticalCtrl) <= -0.8f && Input.GetButtonDown(JumpButton))
                    {
                        GoDown();
                    }
                    else if (Input.GetButtonDown(JumpButton))
                    {
                        jumping = true;
                    }

                     movementX = Input.GetAxisRaw(HorizontalCtrl); 
                }

            }
        }
    }
    protected virtual void LateUpdate()
    {
        UpdateHUD();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(specialStatus == Enum_SpecialStatus.projected)
        {
            /*SetStunStatus();
            if (collision.gameObject.GetComponent<Champion>())
            {
                collision.gameObject.GetComponent<Champion>().SetStunStatus();
            }*/
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Lever")
        {
            Lever lever = collision.gameObject.GetComponent<Lever>();
            trapLever = lever;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Lever")
        {
            trapLever = null;
        }
    }

    protected void DynamicFall()
    {
        if (rb != null && rb.velocity.y < jumpVelocityAtApex && !IsGrounded() && dodgeStatus == Enum_DodgeStatus.ready && attacking == false && rb.gravityScale == 1.0f)
        {
            Fall();
        }
    }
    protected void Fall()
    {
        animator.SetBool("Jump", false);
        rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        animator.SetBool("Fall", true);
        falling = true;
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
                if (Fatigue) {
                    if(inputStatus != Enum_InputStatus.onlyAttack && inputStatus != Enum_InputStatus.blocked)
                    {
                        inputStatus = Enum_InputStatus.onlyMovement;
                        guardStatus = Enum_GuardStatus.noGuard;
                        animator.SetBool("Guarding", false);
                        if (staminablockedTimer > staminaFatigueCooldown && !attacking)
                        {
                            staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
                            inputStatus = Enum_InputStatus.allowed;
                        }
                    }
                }
                else {
                    if (staminablockedTimer > staminaRegenerationCooldown)
                    {
                        staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
                    }
                }
                break;
        }
    }
    protected void RegenerateLimitBreakPerSecond()
    {
        limitBreakGauge = Mathf.Min(limitBreakGauge + limitBreakPerSecond * Time.deltaTime, maxLimitBreakGauge);
    }
    public virtual void IncreaseLimitBreak(float modifier)
    {
        limitBreakGauge = Mathf.Max(Mathf.Min(limitBreakGauge + modifier, maxLimitBreakGauge),0.0f);
    }
    public void ResetLimitBreak()
    {
        immune = false;
        limitBreakGauge = 0.0f;
    }

    protected virtual void PrimaryAttack()
    {
        if(attackToken > 0)
        {
            animator.SetTrigger("PrimaryAttack");

            attackToken--;
        }

    }
    protected virtual void SecondaryAttack()
    {
        animator.SetTrigger("SecondaryAttack");
        ReduceStamina(specialAttack.staminaCost);
        inputStatus = Enum_InputStatus.blocked;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0.0f;
    }
    protected abstract void Ultimate();
    protected abstract void CastHitBox(int attackType);
    protected void StartAttackString()
    {
        ReduceStamina(combo1.staminaCost);
        inputStatus = Enum_InputStatus.onlyAttack;
        movementX = 0;
        movementY = 0;
        attacking = true;
        rb.velocity = Vector2.zero;
    }
    public virtual void MoveOnAttack(int attackID)
    {
        Vector2 force = new Vector2(facing * combo1.movementForce, 0);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    public void ComboOneMoveOnAttack()
    {
        combo1.MoveOnAttack();
    }
    public void ComboOneCastHitBox()
    {
        combo1.CastHitBox();
    }
    protected virtual void EndAttackString()
    {
        if (specialStatus != Enum_SpecialStatus.stun && specialStatus != Enum_SpecialStatus.projected) {
            AllowInputs();
        }
        CheckFatigue();
        attacking = false;
        ResetAttackTokens();
    }

    public virtual void ReduceStamina(float amount)
    {
        if (amount != 0.0f)
        {
            stamina = Mathf.Max(stamina - amount, 0);
            CheckFatigue();
            staminaRegenerationStatus = Enum_StaminaRegeneration.blocked;
            staminablockedTimer = 0.0f;
        }
    }
    public virtual void ReduceHealth(float amount, bool clashPossible = false, Champion attacker = null)
    {
        IncreaseLimitBreak(limitBreakOnDamage); //increase limit break
        if (amount >= health && clashPossible && attacker != null && determination > 1)
        {
            Health = 1;
            Clash(attacker);
        }
        else
            Health = Health - amount;
    }
    public virtual void ConsumeStamina(float amount) //DOESN'T TRIGGER THE STAMINA BLOCKED TIMER
    {
        if (amount != 0.0f)
        {
            stamina = Mathf.Max(stamina - amount, 0);
            CheckFatigue();
        }
    }
    
    public void Clash(Champion attacker)
    {
        Debug.Log("Clash !");
        StartCoroutine(ManagerInGame.GetInstance().ClashRoutine(this, attacker));
    }
    public void ClashMode()
    {
        animator.speed = (1 / Time.timeScale);
        isClashing = true;
        GetComponent<SpriteRenderer>().sortingLayerName = "Clash";
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    public void NormalMode()
    {
        clashClick = 0;
        animator.speed = 1;
        isClashing = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "Default";
    }

    public virtual void ApplyStunLock(int duration) // Player can't execute action while damaged
    {
        if(duration != 0)
        {
            rb.gravityScale = 0.0f;
            stunlockFrameCounter = 0;
            framesToStunLock = duration;
            guardStatus = Enum_GuardStatus.noGuard;
            inputStatus = Enum_InputStatus.blocked;

            animator.SetBool("Damaged", true);
            animator.SetBool("Guarding", false);
        }
    }
    public void ApplyDamage(float dmg, float attackerFacing, int stunLock, Vector2 recoilForce, bool guardBreaker = false, bool clashPossible = false, Champion attacker = null, bool isProjectile = false, bool isUltimate = false)
    {
        if (!Immunity)
        {
            if (guardStatus == Enum_GuardStatus.guarding) 
            {
                if (!guardBreaker && attackerFacing != facing) // attacker is in front of the player and player is guarding, the attacker isn't guard breaking
                {
                    ReduceStamina(dmg * blockStaminaCostMultiplier);
                    dmg = dmg * damageReductionMultiplier;
                    animator.SetTrigger("Blocked");
                    ResetAttackTokens();
                }
                else //the attack is coming from behind or the attack is a guard breaker
                {
                    animator.SetFloat("AttackerFacing", attackerFacing);
                    if(stunLock > 0)
                    {
                        ApplyStunLock(stunLock);
                    }
                    rb.AddForce(recoilForce * attackerFacing, ForceMode2D.Impulse);
                    ResetAttackTokens();
                }
                ReduceHealth(dmg, clashPossible, attacker);
                if (cameraController != null)
                {
                    cameraController.Shake(dmg, 5, 1000);
                }
            }
            else //attacker is behind the player or the player is not guarding
            {
                if (!guardBreaker || isUltimate) //if the attack isn't a guard break or is a guard breaking ultimate
                {
                    animator.SetFloat("AttackerFacing", attackerFacing);
                    if (stunLock > 0)
                    {
                        ApplyStunLock(stunLock);
                    }
                    rb.AddForce(recoilForce * attackerFacing, ForceMode2D.Impulse);
                    ReduceHealth(dmg, clashPossible, attacker);
                    if (cameraController != null)
                    {
                        cameraController.Shake(dmg, 5, 1000);
                    }
                    ResetAttackTokens();
                }
                //else we do nothing, guard breaks are ineffective against non guarding enemies
            }
        }
        else
        {
            if(guardStatus == Enum_GuardStatus.parrying)
            {
                if(attacker != null && !isProjectile)
                {
                    Debug.Log("Parried");
                    rb.velocity = Vector2.zero;
                    IncreaseLimitBreak(limitBreakOnParry);
                    attacker.SetStunStatus(parryStunDuration);
                }
            }
        }
    }

    public void CheckStunLock()
    {
        if (framesToStunLock > 0)
        {
            stunlockFrameCounter++;
            if (stunlockFrameCounter >= framesToStunLock)
            {
                if (specialStatus == Enum_SpecialStatus.projected)
                {
                    SetNormalStatus();
                }
                framesToStunLock = 0;
                stunlockFrameCounter = 0;
                rb.velocity = new Vector2(0, rb.velocity.y);
                attacking = false;
                animator.SetBool("Damaged", false);
                AllowInputs();
                ResetAttackTokens();
            }
        }
        
    }
    protected void CheckFatigue()
    {
        if (stamina == 0)
        {
            fatigued = true;

        }
        else //if(stamina >= baseStamina*0.25f) // Au moins 25% de la stamina
        {
            fatigued = false;
        }
    }
    protected virtual void CheckParry()
    {
        switch (guardStatus)
        {
            case Enum_GuardStatus.noGuard:
                if (Input.GetButtonDown(ParryButton) && inputStatus == Enum_InputStatus.allowed && !fatigued && IsGrounded())
                {
                    parryFrameCounter = 0;
                    guardStatus = Enum_GuardStatus.parrying;
                    inputStatus = Enum_InputStatus.blocked;
                    animator.SetTrigger("Parry");
                    ReduceStamina(parryStaminaCost);
                }
                break;
            case Enum_GuardStatus.parrying:
                parryFrameCounter++;
                if (parryFrameCounter >= parryImmunityStartFrame && parryFrameCounter < parryImmunityEndFrame)
                {
                    immune = true;
                }
                if (parryFrameCounter >= parryImmunityEndFrame)
                {
                    immune = false;
                    guardStatus = Enum_GuardStatus.noGuard;
                    inputStatus = Enum_InputStatus.allowed;
                }
                break;
            default:
                break;
        }
    }
    protected virtual void CheckDodge()
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
                    rb.AddForce(new Vector2(facing * dodgeSpeed, 0), ForceMode2D.Impulse);
                    ReduceStamina(dodgeStaminaCost);
                    dodgeStatus = Enum_DodgeStatus.dodging;
                    inputStatus = Enum_InputStatus.blocked;
                    dodgeToken--;
                    animator.SetBool("Jump", false);
                    animator.SetBool("Fall", false);
                    animator.SetBool("Dodge", true);
                    falling = false;
                }
                break;
            case Enum_DodgeStatus.dodging:
                dodgeFrameCounter++;
                if (dodgeFrameCounter >= dodgeImmunityStartFrame && dodgeFrameCounter < dodgeImmunityEndFrame)
                {
                    immune = true;
                    InvincibilityVisualizer(true);
                }
                if (dodgeFrameCounter >= dodgeImmunityEndFrame)
                {
                    immune = false;
                    InvincibilityVisualizer(false);
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
    protected virtual void CheckUltimate()
    {
        if (limitBreakGauge == maxLimitBreakGauge)
        {
            Ultimate();
        }
    }

    public virtual void Move(float moveX, float moveY)
    {
        float currentSpeed = Fatigue ? speed * fatiguedSpeedReduction : speed;
        if(specialStatus == Enum_SpecialStatus.slow)
            currentSpeed *= slowSpeedReduction;
        //LIMIT DIAGONAL SPEED
        Vector2 movement = new Vector2(moveX, moveY).normalized * currentSpeed;

        //not impeding X movements when aerial
        if (moveX != 0)
        {
            facing = Mathf.Sign(moveX);
            rb.velocity = new Vector2(0, rb.velocity.y);
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
    public void ResetAttackTokens()
    {
        attackToken = maxAttackToken;
    }
    /*
    protected virtual void ApplySpecialEffect(Champion enemy)
    {

        Debug.Log("No special effect on this attack");
    }
    */
    protected void StopMovement(int stopForce)
    {
        movementX = 0;
        movementY = 0;
        if (stopForce == 1 && specialStatus != Enum_SpecialStatus.stun && specialStatus != Enum_SpecialStatus.projected)
        {
            rb.velocity = Vector2.zero;
        }
    }
    protected void ControlCoyote()
    {
        if (!IsGrounded())
        {
            ++coyoteFrameCounter;
        }
        else
        {
            coyoteFrameCounter = 0;
        }
    }
    public virtual void Jump()
    {
        if(rb != null)
        {
            if (coyoteFrameCounter <= coyoteTimeFrames)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(0, jumpHeight * rb.mass), ForceMode2D.Impulse);
                animator.SetBool("Fall", false);
                animator.SetBool("Jump", true);
                coyoteFrameCounter = coyoteTimeFrames + 1;
            }
            else
            {
                if (!IsGrounded())
                {
                    rb.AddForce(new Vector2(0, -jumpHeight * rb.mass), ForceMode2D.Impulse);
                    EnableDiveBox();
                }
            }
        }
    }
    protected void GoDown()
    {
        //Debug.Log("Go down");
        Vector2 centerOne = new Vector2(physicBox.bounds.center.x - (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y);
        Vector2 centerTwo = new Vector2(physicBox.bounds.center.x + (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y);
        float radius = 0.1f;
        Collider2D[] hitOne = Physics2D.OverlapCircleAll(centerOne, radius, LayerMask.GetMask("Obstacle"));
        if (hitOne.Length > 0)
        {
            foreach(Collider2D col in hitOne)
            {
                if (!col.isTrigger && col.GetComponent<PlatformManager>() != null)
                {
                    ignorePlatforms = true;
                    Physics2D.IgnoreCollision(col, physicBox, true);
                    Fall();
                    break;
                }
            }
        }
        else
        {
            Collider2D[] hitTwo = Physics2D.OverlapCircleAll(centerTwo, radius, LayerMask.GetMask("Obstacle"));
            if(hitTwo.Length > 0)
            {
                foreach (Collider2D col in hitOne)
                {
                    if (!col.isTrigger && col.GetComponent<PlatformManager>() != null)
                    {
                        ignorePlatforms = true;
                        Physics2D.IgnoreCollision(col, physicBox, true);
                        Fall();
                        break;
                    }
                }
            }
            else
            {
                ignorePlatforms = false;
            }
        }
    }
    public virtual bool IsGrounded()
    {
        //returns true if collides with an obstacle underneath object
        Vector2 centerOne = new Vector2(physicBox.bounds.center.x - (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y);
        Vector2 centerTwo = new Vector2(physicBox.bounds.center.x + (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y);
        float radius = 0.1f;

        if ((Physics2D.OverlapCircle(centerOne, radius, LayerMask.GetMask("Obstacle")) || Physics2D.OverlapCircle(centerTwo, radius, LayerMask.GetMask("Obstacle"))) && !ignorePlatforms)
        {
            animator.SetBool("Fall", false);
            DisableDiveBox();
            falling = false;
            return true;
        }
        return false;
    }

    public void AllowInputs()   //activated in the animation controller
    {
        if(specialStatus != Enum_SpecialStatus.stun && specialStatus != Enum_SpecialStatus.projected)
        {
            Debug.Log(specialStatus);
            rb.gravityScale = 1.0f;
            inputStatus = Enum_InputStatus.allowed;
            ResetAttackTokens();
        }
        
    }

    public IEnumerator ProcDivineShield(float time)
    {
        if (!dead)
        {
            immune = true;
            inputStatus = Enum_InputStatus.onlyMovement;
            specialStatus = Enum_SpecialStatus.normal;
            rb.velocity = Vector2.zero;
            InvincibilityVisualizer();
            rb.gravityScale = 1.0f;
            yield return new WaitForSeconds(time);
            rb.gravityScale = 1.0f;
            inputStatus = Enum_InputStatus.allowed;
            immune = false;
            InvincibilityVisualizer();
        }
    }

    protected void DisableDiveBox()
    {
       if(diveBox != null)
       {
            diveBox.GetComponent<Hitbox>().enabled = false;
            diveBox.enabled = false;
       }
    }
    protected void EnableDiveBox()
    {
        if (diveBox != null)
        {
            diveBox.GetComponent<Hitbox>().enabled = true;
            diveBox.enabled = true;
        }
    }

    protected void InvincibilityVisualizer(bool enable = false)
    {
        if (immune)
        {
            sr.color = Color.yellow;
        }
        else
        {
            sr.color = Color.white;
        }
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
    public void SetSecondaryAttackButton(string SAButton)
    {
        SecondaryAttackButton = SAButton;
    }
    public void SetPowerUpButton(string PUButton)
    {
        PowerUpButton = PUButton;
    }
    public void SetGuardButton(string GButton)
    {
        GuardButton = GButton;
    }
    public void SetParryButton(string PButton)
    {
        ParryButton = PButton;
    }
    public void SetActionButton(string AButton)
    {
        ActionButton = AButton;
    }

    public void UpdateHUD()
    {
        float a = healthSlider.value;
        healthSlider.value = health;
        staminaSlider.value = stamina;

        limitBreakSlider.value = limitBreakGauge;
        float b = healthSlider.value;
        if (a != b) //si recu des degats, barre colorée supplémentaire
        {
            timerDamageHUD = 40;
            playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").Find("Test").GetComponent<Image>().color = new Color(255, 155, 0);
            playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").Find("Test").GetComponent<RectTransform>().sizeDelta = new Vector2((a - b) * 1.4f, 10);
            if (playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").Find("Test").GetComponent<RectTransform>().anchoredPosition.x > 0)
            {
                playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").Find("Test").GetComponent<RectTransform>().anchoredPosition = new Vector2((a - b) * 1.4f, 0);
            }
            else
            {
                playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").Find("Test").GetComponent<RectTransform>().anchoredPosition = new Vector2(-(a - b) * 1.4f, 0);
            }
        }
        else
        {
            if (timerDamageHUD < 0) // au bout de x ticks, on fait disparaitre la barre
            {
                playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").Find("Test").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            }
        }

        timerDamageHUD -= 1;
        ultiImageSlider.fillAmount = 0.75f;
        ChangeColorHealthSlider();

        //PowerUpAvailable(true); //changer la transparence du powerup (1 quand dispo et 0.4 quand en charge)
        UltiAvailable(true);
    }

    public void ChangeColorHealthSlider()
    {
        if (determination == 2)
            playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(255, 155, 0);
        else if (determination == 1)
            playerHUD.transform.Find("HealthSlider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(255, 0, 0);
    }
    /*public void PowerUpAvailable(bool b)
    {
        float a = 0.4f;
        if (b)
            a = 1f;
        playerHUD.transform.Find("PowerUpImage").Find("AbilityImage1").GetComponent<Image>().color = new Color(255, 255, 255, a);
    }*/
    public void UltiAvailable(bool b)
    {
        float a = 0.4f;
        if (b)
            a = 1f;
        playerHUD.transform.Find("UltiImage").Find("AbilityImage2").GetComponent<Image>().color = new Color(255, 255, 255, a);
    }

    public void SetTrapLever(Lever lever)
    {
        trapLever = lever;
    }

    public float Facing
    {
        get
        {
            return facing;
        }
        set
        {
            facing = value;
            animator.SetFloat("FaceX", facing);
        }
    }
    public float Stamina
    {
        get
        {
            return stamina;
        }
		set{ 
			stamina = value;
		}
    }
    public float BaseStamina
    {
        get
        {
            return baseStamina;
        }
    }
    public float LimitBreakGauge
    {
        get
        {
            return limitBreakGauge;
        }
    }
    public float LimitBreakOnDamage
    {
        get
        {
            return limitBreakOnDamage;
        }
    }
    public float LimitBreakOnHit
    {
        get
        {
            return limitBreakOnHit;
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
    public float BaseHealth
    {
        get
        {
            return baseHealth;
        }
    }
    public float Health
    {
        get
        {
            return health;
        }
		set{ 
			health = Mathf.Min(Mathf.Max(value, 0.0f),BaseHealth);
            if (health == 0)
            {
                inputStatus = Enum_InputStatus.blocked;
                foreach (AnimatorControllerParameter parameter in animator.parameters)
                {
                    if (parameter.type == AnimatorControllerParameterType.Bool)
                    {
                        animator.SetBool(parameter.name, false);
                    }
                }
                dead = true;
                playerBox.enabled = false;
                StopMovement(1);
                Debug.Log(transform.parent.name + " died");

                //TO DO : find a way to use the deadLayer variable since this doesn't work
                /*if(deadLayer == null)
                {
                    deadLayer = LayerMask.NameToLayer("Dead");

                }
                gameObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(deadLayer));
                */
                //this works but uses a string
                gameObject.layer = LayerMask.NameToLayer("Dead");
            }
        }
    }
    public bool Immunity
    {
        get
        {
            return immune;
        }
        set
        {
            immune = value;
        }
    }

    public Enum_GuardStatus GuardStatus
    {
        get
        {
            return guardStatus;
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
    public Animator Animator
    {
        get { return animator; }
    } 
    public Rigidbody2D RB
    {
        get { return rb; }
    }
    public LayerMask HitBoxLayer
    {
        get { return hitBoxLayer; }
    }
    public Vector2 Position
    {
        get { return transform.position; }
    }
    public int GuardBreakDamage
    {
        get
        {
            return guardBreakDamage;
        }
    }
    public int GuardBreakStunLock
    {
        get
        {
            return guardBreakstunLock;
        }
    }
    public Vector2 GuardBreakRecoilForce
    {
        get
        {
            return guardBreakRecoilForce;
        }
    }

    public void SetStunEffects()
    {
        inputStatus = Enum_InputStatus.blocked;
        guardStatus = Enum_GuardStatus.noGuard;
        animator.SetBool("Guarding", false);
        animator.SetBool("Jump", false);
    }

    public void SetProjectedStatus(float attackerFacing, Vector2 projectionForce, float duration = DEFAULT_EFFECT_DURATION)
    {
        if (!immune && specialStatus != Enum_SpecialStatus.projected)
        {
            specialStatus = Enum_SpecialStatus.projected;
            SetStunEffects();
            Facing = attackerFacing != 0 ? -attackerFacing : -1.0f;
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0.0f;
            animator.SetBool("Projected", true);
            rb.AddForce(new Vector2(projectionForce.x * attackerFacing, projectionForce.y), ForceMode2D.Impulse);
            StartCoroutine(EffectCoroutine(duration));
            projectedCoroutine = StartCoroutine(ProjectionCoroutine());
        }
    }
    public void SetNormalStatus()
    {
        if(projectedCoroutine != null)
        {
            StopCoroutine(projectedCoroutine);
        }
        animator.SetBool("Projected", false);
        animator.SetBool("Stunned", false);
        specialStatus = Enum_SpecialStatus.normal;
        speed = baseSpeed;
        AllowInputs();
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

        //Debug.Log("Normal is the new black");
    }
    public void SetStunStatus(float duration = DEFAULT_EFFECT_DURATION)
    {
        if(!immune && specialStatus != Enum_SpecialStatus.stun)
        {
            Debug.Log("Stunned");
            rb.velocity = Vector2.zero;
            animator.SetBool("Projected", false);
            animator.SetBool("Stunned", true);
            rb.gravityScale = 1.0f;
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
            specialStatus = Enum_SpecialStatus.stun;
            SetStunEffects();
            StartCoroutine(EffectCoroutine(duration));
        }
        
    }
    public void SetPoisonStatus(float poisonDamage = 2.0f, float duration = DEFAULT_EFFECT_DURATION)
    {
        //Debug.Log("POISON");
        if (!immune)
        {
            specialStatus = Enum_SpecialStatus.poison;
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
            StartCoroutine(PoisonCoroutine(poisonDamage));
            StartCoroutine(EffectCoroutine(duration));
        }
    }
    public void SetSlowStatus(float duration = DEFAULT_EFFECT_DURATION, float slowRatio = 0.75f)
    {
        //Debug.Log("SLOW");
        if (!immune)
        {
            specialStatus = Enum_SpecialStatus.slow;
            speed = baseSpeed * slowRatio;
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f);
            StartCoroutine(EffectCoroutine(duration));
        }
    }

    public bool IsJumping()
    {
        return (IsGrounded() && !falling);
    }
    public bool IsFalling()
    {
        return falling;
    }
    public bool IgnorePlatforms
    {
        get
        {
            return ignorePlatforms;
        }
        set
        {
            ignorePlatforms = value;
        }
    }
    public ParticleSystem PowerUpParticleSystem
    {
        get
        {
            return powerUpParticleSystem;
        }
    }

    IEnumerator EffectCoroutine(float duration)
    {
        Enum_SpecialStatus startingEffect = specialStatus;
        yield return new WaitForSeconds(duration);
        if(specialStatus == startingEffect || specialStatus == Enum_SpecialStatus.normal)
        {
            SetNormalStatus();
        }
        
    }
    IEnumerator ProjectionCoroutine()
    {
        while(specialStatus == Enum_SpecialStatus.projected)
        {
            SetStunEffects();
            Vector2 wallDetectorPosition = new Vector2(physicBox.bounds.center.x + physicBox.bounds.extents.x * -facing, physicBox.bounds.center.y);
            Collider2D hitObstacle = Physics2D.OverlapCircle(wallDetectorPosition, 0.5f, LayerMask.GetMask("Obstacle"));
            Collider2D hitPlayer = Physics2D.OverlapCircle(wallDetectorPosition, 0.5f, LayerMask.GetMask("Player"));
            if(hitObstacle == null /*&& hitPlayer == null*/)
            {
                yield return null;
            }
            else
            {
                if(hitObstacle != null)
                {
                    SetStunStatus();
                }
                else
                {
                    if(hitPlayer != null)
                    {
                        Champion other = hitPlayer.GetComponent<Champion>();
                        Debug.Log(other.gameObject.name);
                        if(other != this && other != null)
                        {
                            SetStunStatus();
                            other.SetStunStatus();
                        }
                    }
                }
                
            }
        }
    }
    IEnumerator StunCoroutine(float duration)
    {
        SetStunEffects();
        Enum_SpecialStatus startingEffect = specialStatus;
        yield return new WaitForSeconds(duration);
        if (specialStatus == startingEffect || specialStatus == Enum_SpecialStatus.normal)
        {
            SetNormalStatus();
        }
    }

    IEnumerator PoisonCoroutine(float poisonDamage)
    {
        while (specialStatus == Enum_SpecialStatus.poison)
        {
            ReduceHealth(poisonDamage);
            yield return new WaitForSeconds(1);
        }
    }
    public void UltimateCameraEffect()
    {
        StartCoroutine(ManagerInGame.GetInstance().UltimateCameraEffect(transform.position, zoomWaitDuration));
    }
}
