using System;
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

    [SerializeField] protected float baseHealth = 200;
    [SerializeField] public int determination = 3;
    [SerializeField] protected float baseSpeed = 10;
    [SerializeField] protected LayerMask deadLayer;

    [Header("HUDSettings")]
    [SerializeField] public CanvasGroup playerHUD;
    [SerializeField] protected GameObject damageDisplayPrefab;

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
    [SerializeField] protected float damageReductionMultiplier = 0.1f;
    [SerializeField] protected float blockStaminaCostMultiplier = 1.4f;

    [Header("Stamina Settings")]
    [SerializeField] public float baseStamina = 100f;
    [SerializeField] public float staminaRegenerationPerSecond = 15f;
    [SerializeField] protected float staminaRegenerationCooldown = 1.5f;
    [SerializeField] protected float staminaFatigueCooldown = 3.0f;
    [SerializeField] protected float primaryFireStaminaCost = 20f;
    [SerializeField] protected float secondaryFireStaminaCost = 40f;

    [Header("Limit Break Settings")]
    [SerializeField] public float maxLimitBreakGauge = 100;
    [SerializeField] public float limitBreakPerSecond = 0.40f;
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

    [Header("Narrator Quotes")]
    [SerializeField] protected AudioClip[] ultimateQuotes;

    [Header("Sound Settings")]
    [SerializeField] protected Sound[] soundEffects;


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
    [NonSerialized] public Animator animator;
    protected Vector2 savedVelocity;
    protected Vector2 wallColliderPosition;
    protected Collider2D playerBox;
    protected Collider2D physicBox;
    protected Collider2D diveBox;
    protected bool jumping, falling = false, immune = true, parrying = false, fatigued = false, attacking = false, dead = false, isClashing=false;
    protected Enum_InputStatus inputStatus = Enum_InputStatus.blocked;
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
    protected PopupDamage damageDisplay;

    [System.NonSerialized] public int clashClick=0;

    protected Slider healthSlider;
    protected Slider staminaSlider;
    protected Slider limitBreakSlider;
    protected int timerDamageHUD = 40;
    protected Image ultiImageSlider;

    protected SpriteRenderer sr;
    protected CameraControl cameraController;
    protected AudioVolumeManager audioVolumeManager;
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

    protected bool lowHealthOnce = false;
    protected bool lowStaminaOnce = false;
    protected bool ultOnce = false;
    protected AudioSource audioSource; // remove when narrator is fully implemented
    [NonSerialized] public Transform originalParent;
    protected X360_controller controller;
    [NonSerialized] public bool hardBlock = true;
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(new Vector3(physicBox.bounds.center.x - (physicBox.bounds.extents.x/2) * facing, physicBox.bounds.min.y, 0), 0.2f); //to visualize the ground detector
        //Gizmos.DrawSphere(new Vector3(physicBox.bounds.center.x + (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y,0), 0.2f);
        //Gizmos.DrawWireSphere(new Vector3(physicBox.bounds.center.x + physicBox.bounds.extents.x * -facing, physicBox.bounds.center.y, 0),0.5f);
       // Gizmos.DrawWireCube(new Vector2(physicBox.bounds.center.x + physicBox.bounds.extents.x * -facing, physicBox.bounds.center.y), new Vector3(1.0f, 1.0f, 1.0f));
    }

    protected void OnEnable()
    {
        
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
        limitBreakSlider = playerHUD.transform.Find("UltiSlider").GetComponent<Slider>();
        //ultiImageSlider = playerHUD.transform.Find("UltiSlider").Find("RadialSliderImage").GetComponent<Image>();
        UpdateHUD();
        ResetAttackTokens();

        cameraController = Camera.main.GetComponent<CameraControl>();
        sr = GetComponent<SpriteRenderer>();
        if(powerUpParticleSystem != null)
        {
            powerUpParticleSystem.Stop();
        }
        if (ultimateParticleSystem != null)
        {
            ultimateParticleSystem.Stop();
        }

        audioVolumeManager = AudioVolumeManager.GetInstance();
        audioSource = GetComponent<AudioSource>(); // remove when narrator is fully implemented
        ManagerInGame gameManager = ManagerInGame.GetInstance();
        foreach (Sound s in soundEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            if(s.name == "Ultimate" || s.name == "Death")
            {
                gameManager.AddAudioSource(s.source);
            }
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.volume = audioVolumeManager.SoundEffectVolume;
            s.source.spatialBlend = 0.0f;
        }
        originalParent = transform.parent;
    }
    protected void FixedUpdate()
    {
        if (Health >= baseHealth * 0.20f)
        {
            lowHealthOnce = false;
        }
        if (specialStatus != Enum_SpecialStatus.projected || dead)
        {
            DynamicFall();
        }
        if(!hardBlock)
        {
            if (jumping && guardStatus == Enum_GuardStatus.noGuard)
            {
                Jump();
                jumping = false;
            }
            Move(movementX, movementY);
        }
        
        if (dead)
        {
            
            if(!IsGrounded())
            {
                Fall();
            }
        }
    }
    protected virtual void Update()
    {
        if (!dead)
        {
            if(controller != null && !hardBlock)
            {
                RegenerateStaminaPerSecond();
                RegenerateLimitBreakPerSecond();
                ControlCoyote();
                CheckStunLock();
                CheckFatigue();
                if (isClashing)
                {
                    if (controller.GetButtonDown("A"))
                    {
                        clashClick++;
                    }
                    return;
                }
                CheckDodge();
                CheckParry();

                if (InputStatus == Enum_InputStatus.blocked && specialStatus != Enum_SpecialStatus.projected && specialStatus != Enum_SpecialStatus.stun)
                {
                    StopMovement(0);
                }
                else
                {
                    if (controller.GetTrigger_L() >= 0.6f && controller.GetTrigger_R() >= 0.6f)
                    {
                        CheckUltimate();
                    }
                    else if (controller.GetTrigger_R() >= 0.6f && powerUp != null && powerUp.PowerUpStatus == Enum_PowerUpStatus.available)
                    {
                        if (powerUp != null)
                        {
                            powerUp.ActivatePowerUp();
                        }
                    }

                    if (controller.GetButtonDown("Y") && InputStatus != Enum_InputStatus.onlyMovement && !Fatigue && guardStatus == Enum_GuardStatus.noGuard)
                    {
                        SecondaryAttack();
                    }

                    // Action button
                    if (controller.GetButtonDown("RB") && trapLever != null && InputStatus != Enum_InputStatus.onlyMovement)
                    {
                        Debug.Log("ActionButton");
                        if (trapLever.canEngage)
                        {
                            trapLever.Engage();
                        }
                    }

                    if (IsGrounded() && InputStatus != Enum_InputStatus.onlyMovement)
                    {
                        if (guardStatus == Enum_GuardStatus.noGuard && !Fatigue)
                        {
                            if (controller.GetButtonDown("X"))
                            {
                                PrimaryAttack();
                            }
                        }

                        if (controller.GetTrigger_L() != 0 && guardStatus != Enum_GuardStatus.parrying && !Fatigue)
                        {
                            guardStatus = Enum_GuardStatus.guarding;
                            animator.SetBool("Guarding", true);
                        }
                        if (controller.GetTrigger_L() == 0)
                        {
                            guardStatus = Enum_GuardStatus.noGuard;
                            animator.SetBool("Guarding", false);
                        }
                    }

                    if (InputStatus != Enum_InputStatus.onlyAttack)
                    {
                        if (controller.GetStick_L().Y <= -0.8f && controller.GetButtonDown("A"))
                        {
                            if (IsGrounded())
                            {
                                GoDown();
                            }
                            else
                            {
                                GuardBreak();
                            }

                        }
                        else if (controller.GetButtonDown("A"))
                        {
                            jumping = true;
                        }

                        movementX = Mathf.Ceil(controller.GetStick_L().X);
                    }

                }
            }
        }
    }
    protected virtual void LateUpdate()
    {
        UpdateHUD();
        
        if(Health < baseHealth * 0.20f && !lowHealthOnce)
        {
            PlaySoundEffect("LowHealth");
            lowHealthOnce = true;
        }
        if(Fatigue && !lowStaminaOnce)
        {
            PlaySoundEffect("LowStamina");
            lowStaminaOnce = true;
            
        }
        if(limitBreakGauge == maxLimitBreakGauge && !ultOnce)
        {
            PlaySoundEffect("UltReady");
            ultOnce = true;
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

    public void PlaySoundEffect(string name)
    {
        Sound s = Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound " + name + " not found");
            return;
        }
        s.source.pitch = s.pitch;
        s.source.volume = audioVolumeManager.SoundEffectVolume;
        s.source.Play();
    }

    public void PlaySoundEffectRandomPitch(string name)
    {
        Sound s = Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound " + name + " not found");
            return;
        }
        float pitch = UnityEngine.Random.Range(0.9f, 1.8f);
        s.source.pitch = pitch;
        s.source.volume = audioVolumeManager.SoundEffectVolume;
        s.source.Play();
    }

    protected void StopSoundEffect(string name)
    {
        Sound s = Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound " + name + " not found");
            return;
        }
        s.source.Stop();
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
        if (!dead)
        {
            animator.SetBool("Fall", true);
            falling = true;
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
                if (Fatigue) {
                    if(inputStatus != Enum_InputStatus.onlyAttack && inputStatus != Enum_InputStatus.blocked)
                    {
                        if (specialStatus != Enum_SpecialStatus.projected && specialStatus != Enum_SpecialStatus.stun)
                        {
                            inputStatus = Enum_InputStatus.onlyMovement;
                        }
                        guardStatus = Enum_GuardStatus.noGuard;
                        animator.SetBool("Guarding", false);
                        if (staminablockedTimer > staminaFatigueCooldown && !attacking)
                        {
                            staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
                            if(specialStatus != Enum_SpecialStatus.projected && specialStatus != Enum_SpecialStatus.stun)
                            {
                                inputStatus = Enum_InputStatus.allowed;
                            }
                            
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
        ultOnce = false;
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
        inputStatus = Enum_InputStatus.blocked;
        animator.SetTrigger("SecondaryAttack");
        ReduceStamina(specialAttack.staminaCost);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0.0f;
    }
    protected abstract void Ultimate();

    public void EndUltLoop()
    {
        animator.SetTrigger("UltLaunch");
    }

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
        if(attacker != null && amount > 8.0f)
        {
            attacker.IncreaseLimitBreak(attacker.limitBreakOnHit);
        }
        
        if(damageDisplay == null)
        {
            InstantiateDamageDisplay();
        }
        damageDisplay.SetText(amount);
        if (amount >= health && clashPossible && attacker != null && determination > 1)
        {
            Narrator.Instance.Clash();
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
        animator.ResetTrigger("PrimaryAttack");
        animator.ResetTrigger("SecondaryAttack");
        animator.speed = (1 / Time.timeScale);
        isClashing = true;
        SpriteRenderer playerNumber = originalParent.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
       // controller.AddRumble(0.1f, new Vector2(0.4f, 0.4f), 0.1f);

        sprite.sortingLayerName = "Clash";
        sprite.sortingOrder = 10;
        playerNumber.sortingLayerName = "Clash";
        playerNumber.sortingOrder = 10;
    }
    public void NormalMode()
    {
        clashClick = 0;
        animator.speed = 1;
        isClashing = false;

        SpriteRenderer playerNumber = originalParent.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        // controller.AddRumble(0.1f, new Vector2(0.4f, 0.4f), 0.1f);

        sprite.sortingLayerName = "Clash";
        sprite.sortingOrder = 10;
        playerNumber.sortingLayerName = "Clash";
        playerNumber.sortingOrder = 10;
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
                    Narrator.Instance.Guard();
                    ReduceStamina(dmg * blockStaminaCostMultiplier);
                    dmg = dmg * damageReductionMultiplier;
                    animator.SetTrigger("Blocked");
                    ResetAttackTokens();
                    controller.AddRumble(0.3f, new Vector2(0.1f, 0.1f), 0.3f);
                }
                else //the attack is coming from behind or the attack is a guard breaker
                {
                    Narrator.Instance.Attack();
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
                controller.AddRumble(0.3f, new Vector2(0.5f, 0.5f), 0.3f);
            }
            else //attacker is behind the player or the player is not guarding
            {
                if (!guardBreaker || isUltimate) //if the attack isn't a guard break or is a guard breaking ultimate
                {
                    Narrator.Instance.Attack();
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
                    controller.AddRumble(0.3f, new Vector2(0.5f, 0.5f), 0.3f);
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
                    Narrator.Instance.Parry();
                    Debug.Log("Parried");
                    rb.velocity = Vector2.zero;
                    IncreaseLimitBreak(limitBreakOnParry);
                    attacker.SetStunStatus(parryStunDuration);
                    PlaySoundEffect("Parry");
                    controller.AddRumble(0.3f, new Vector2(0.9f, 0.9f), 0.3f);
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
            lowStaminaOnce = false;
        }
    }
    protected virtual void CheckParry()
    {
        switch (guardStatus)
        {
            case Enum_GuardStatus.noGuard:
                if (controller.GetButtonDown("LB") && inputStatus == Enum_InputStatus.allowed && !fatigued && IsGrounded())
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
                    if (specialStatus != Enum_SpecialStatus.projected && specialStatus != Enum_SpecialStatus.stun)
                    {
                        inputStatus = Enum_InputStatus.allowed;
                    }
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
                if (controller.GetButtonDown("B") && inputStatus == Enum_InputStatus.allowed &&
                    guardStatus == Enum_GuardStatus.noGuard && !fatigued && dodgeToken > 0 && specialStatus != Enum_SpecialStatus.projected)
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
                }
                if (dodgeFrameCounter >= dodgeImmunityEndFrame)
                {
                    immune = false;
                }
                if (dodgeFrameCounter >= dodgeFrames)
                {
                    dodgeFrameCounter = dodgeFrames;
                    if(specialStatus != Enum_SpecialStatus.projected && specialStatus != Enum_SpecialStatus.stun)
                    {
                        rb.velocity = Vector2.zero;
                        inputStatus = Enum_InputStatus.allowed;
                        dodgeStatus = Enum_DodgeStatus.ready;
                    }
                    animator.SetBool("Dodge", false);
                    playerBox.enabled = true;
                    
                }
                break;
        }
    }
    protected virtual void CheckUltimate()
    {
        if (limitBreakGauge == maxLimitBreakGauge)
        {
            Narrator.Instance.Ultimate();
            Ultimate();
        }
    }

    public virtual void Move(float moveX, float moveY)
    {
        if(specialStatus != Enum_SpecialStatus.projected && specialStatus != Enum_SpecialStatus.stun){
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
		if(rb != null && specialStatus != Enum_SpecialStatus.stun)
		{
			if (coyoteFrameCounter <= coyoteTimeFrames)
			{
				rb.velocity = Vector2.zero;
				rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
				animator.SetBool("Fall", false);
				animator.SetBool("Jump", true);
				coyoteFrameCounter = coyoteTimeFrames + 1;
			}
		}
	}

    protected virtual void GuardBreak()
    {
        if (!IsGrounded())
        {
            rb.AddForce(new Vector2(0, -jumpHeight), ForceMode2D.Impulse);
            EnableDiveBox();
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
		if (!ignorePlatforms)
		{
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
		}
		return false;
	}

	public void AllowInputs()   //activated in the animation controller
	{
		if(specialStatus != Enum_SpecialStatus.stun && specialStatus != Enum_SpecialStatus.projected)
		{
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
			rb.gravityScale = 1.0f;
			yield return new WaitForSeconds(time);
			rb.gravityScale = 1.0f;
			inputStatus = Enum_InputStatus.allowed;
			immune = false;
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
		//ultiImageSlider.fillAmount = 0.75f;
		ChangeColorHealthSlider();

		//PowerUpAvailable(true); //changer la transparence du powerup (1 quand dispo et 0.4 quand en charge)
		//UltiAvailable(true);
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
		set{ 
			limitBreakGauge = Mathf.Min(Mathf.Max(value, 0.0f),maxLimitBreakGauge);
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
            if (health <= 0.0f)
            {
                Narrator.Instance.Death();
                Death();
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
        dodgeStatus = Enum_DodgeStatus.ready;
        animator.ResetTrigger("PrimaryAttack");
        animator.ResetTrigger("SecondaryAttack");
        animator.SetBool("Dodge", false);
        playerBox.enabled = true;
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
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0.0f;
            animator.ResetTrigger("PrimaryAttack");
            animator.ResetTrigger("SecondaryAttack");
            animator.SetBool("Projected", true);
            rb.AddForce(new Vector2(projectionForce.x * attackerFacing, projectionForce.y), ForceMode2D.Impulse);
            StartCoroutine(EffectCoroutine(duration));
            projectedCoroutine = StartCoroutine(ProjectionCoroutine());
            controller.AddRumble(0.1f, new Vector2(0.5f, 0.5f), 0.1f);
        }
    }
    public void SetNormalStatus()
    {
        if(projectedCoroutine != null && specialStatus == Enum_SpecialStatus.projected)
        {
            StopCoroutine(projectedCoroutine);
            rb.velocity = Vector2.zero;
        }
        animator.SetBool("Projected", false);
        animator.SetBool("Stunned", false);
        specialStatus = Enum_SpecialStatus.normal;
        speed = baseSpeed;
        AllowInputs();

        //Debug.Log("Normal is the new black");
    }
    public void SetStunStatus(float duration = DEFAULT_EFFECT_DURATION)
    {
        if(!immune && specialStatus != Enum_SpecialStatus.stun)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("Projected", false);
            animator.SetBool("Stunned", true);
            rb.gravityScale = 1.0f;
            specialStatus = Enum_SpecialStatus.stun;
            SetStunEffects();
            StartCoroutine(EffectCoroutine(duration));
            controller.AddRumble(0.1f, new Vector2(0.9f, 0.9f), 0.1f);
        }
        
    }
    public void SetPoisonStatus(float poisonDamage = 2.0f, float duration = DEFAULT_EFFECT_DURATION)
    {
        //Debug.Log("POISON");
        if (!immune)
        {
            specialStatus = Enum_SpecialStatus.poison;
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
            Vector2 size = new Vector2(0.8f, 0.4f);
            Vector2 pointA;
            Vector2 pointB;
            if(facing < 0)
            {
                pointA = new Vector2(physicBox.bounds.max.x, physicBox.bounds.min.y + physicBox.bounds.extents.y/6);
                pointB = new Vector2(physicBox.bounds.max.x + physicBox.bounds.extents.x, physicBox.bounds.max.y);
            }
            else
            {
                pointA = new Vector2(physicBox.bounds.min.x, physicBox.bounds.min.y + physicBox.bounds.extents.y / 6);
                pointB = new Vector2(physicBox.bounds.min.x - physicBox.bounds.extents.x, physicBox.bounds.max.y);
            }
            Collider2D hitObstacle = Physics2D.OverlapArea(pointA, pointB,LayerMask.GetMask("Obstacle"));
            Collider2D[] hitPlayer = Physics2D.OverlapAreaAll(pointA, pointB, LayerMask.GetMask("Player"));
            
            if(hitObstacle == null && hitPlayer.Length < 2 )
            {
                yield return null;
            }
            else 
            {
                if(hitObstacle != null)
                {
                    SetStunStatus();
                }
                else if (hitPlayer.Length > 1)
                {
                    foreach (Collider2D col in hitPlayer)
                    {
                        Champion temp = col.GetComponent<Champion>();
                        if (temp != this)
                        {
                            SetStunStatus();
                            temp.SetStunStatus();
                        }
                    }
                }
                else
                {
                    yield return null;
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
            if(poisonDamage >= Health)
            {
                Health = 1;
            }
            else
            {
                ReduceHealth(poisonDamage);
            } 
            yield return new WaitForSeconds(1);
        }
    }
    public void UltimateCameraEffect()
    {
        int id = 0;
        if (audioSource != null && ultimateQuotes.Length > 0)
        {
            id = UnityEngine.Random.Range(0, ultimateQuotes.Length);
            audioSource.PlayOneShot(ultimateQuotes[id], audioVolumeManager.VoiceVolume);
            StartCoroutine(ManagerInGame.GetInstance().UltimateCameraEffect(transform.position, ultimateQuotes[id].length, this));
        }
    }

    public void Death()
    {
        inputStatus = Enum_InputStatus.blocked;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
        }
        if (!IsGrounded())
        {
            Fall();
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
        DeathBehaviour();
    }

    public virtual void DeathBehaviour()
    {
        animator.SetBool("Dead", true);
        StartCoroutine(ManagerInGame.GetInstance().LastDeathCameraEffect(transform.position, 2.0f));
        if(powerUpParticleSystem != null)
        {
            powerUpParticleSystem.Play();
        }
    }

    protected void InstantiateDamageDisplay()
    {
        GameObject temp = Instantiate(damageDisplayPrefab);
        temp.transform.SetParent(playerHUD.transform.parent);
        damageDisplay = temp.GetComponent<PopupDamage>();
        damageDisplay.Target = this;
    }
    void TestRumble()
    {
        //                timer            power         fade
        //controller.AddRumble(1.0f, new Vector2(0.9f, 0.9f), 0.5f);
        controller.AddRumble(0.5f, new Vector2(0.5f, 0.5f), 0.2f);
    }

    public void SetController(int index)
    {
        controller = ControllerManager.Instance.GetController(index);
    }

    public X360_controller Controller
    {
        get
        {
            return controller;
        }
    }
}
