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

    [SerializeField] protected int baseHealth = 100;
    [SerializeField] protected int determination = 3;
    [SerializeField] protected float speed = 10;
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

    [Header("Status Settings")]
    [SerializeField] protected float fatiguedSpeedReduction = 1 / 1.2f;
    [SerializeField] protected float slowSpeedReduction = 1 / 1.2f;

    [Header("Attack Settings")]
    [SerializeField] protected int comboOneDamage = 10;
    [SerializeField] protected float primaryAttackMovementForce = 2;
    [SerializeField] protected LayerMask hitBoxLayer;
    [SerializeField] protected int maxAttackToken = 1;

    [Header("Guard Break Settings")]
    [SerializeField] protected int guardBreakDamage = 5;
    [SerializeField] protected int guardBreakstunLock = 15;
    [SerializeField] protected Vector2 guardBreakRecoilForce;

    public Attack combo1;

    [Header("Combo1Settings")]
    [SerializeField] protected Vector2 comboOneOffset = new Vector2(0, 0);
    [SerializeField] protected Vector2 comboOneSize = new Vector2(1, 1);
    [SerializeField] protected int comboOneStunLock = 5;
    [SerializeField] protected Vector2 comboOneRecoilForce;



    protected int health, framesToStunLock = 0, stunlockFrameCounter = 0;
    protected float stamina, staminablockedTimer, dodgeTimeStart, limitBreakGauge;
    protected int dodgeFrameCounter = 0;
    protected int coyoteFrameCounter = 0;
    protected int parryFrameCounter = 0;
    protected int attackToken = 1;
    protected float distToGround, facing;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 savedVelocity;
    protected Collider2D playerBox;
    protected Collider2D physicBox;
    protected Collider2D diveBox;
    protected bool jumping, immune = false, parrying = false, fatigued = false, attacking = false, dead = false;
    protected Enum_InputStatus inputStatus = Enum_InputStatus.allowed;
    protected Enum_DodgeStatus dodgeStatus = Enum_DodgeStatus.ready;
    protected Enum_StaminaRegeneration staminaRegenerationStatus = Enum_StaminaRegeneration.regenerating;
    protected Enum_GuardStatus guardStatus = Enum_GuardStatus.noGuard;
    protected Enum_SpecialStatus specialStatus = Enum_SpecialStatus.normal;
    protected float movementX, movementY;
    protected PowerUp powerUp;

    protected Slider healthSlider;
    protected Slider staminaSlider;

    protected Image ultiImageSlider;


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

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(new Vector3(physicBox.bounds.center.x - (physicBox.bounds.extents.x/2) * facing, physicBox.bounds.min.y, 0), 0.2f); //to visualize the ground detector
        //Gizmos.DrawSphere(new Vector3(physicBox.bounds.center.x + (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y,0), 0.2f);
    }
    protected void Awake()
    {
        facing = (transform.parent.gameObject.name == "Player1" || transform.parent.gameObject.name == "Player3") ? 1.0f : -1.0f;
        coyoteFrameCounter = coyoteTimeFrames; //to allow players to fall at the beginning
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
        diveBox = transform.Find("DiveBox").GetComponentInChildren<Collider2D>();
        powerUp = GetComponent<PowerUp>();

        combo1.SetUser(this);

        playerHUD.alpha = 1;
        healthSlider = playerHUD.transform.Find("HealthSlider").GetComponent<Slider>();
        staminaSlider = playerHUD.transform.Find("StaminaSlider").GetComponent<Slider>();

        ultiImageSlider = playerHUD.transform.Find("UltiImage").Find("RadialSliderImage").GetComponent<Image>();
        UpdateHUD();
        ResetAttackTokens();

        cameraController = Camera.main.GetComponent<CameraControl>();
    }
    protected void FixedUpdate()
    {
        if(specialStatus != Enum_SpecialStatus.projected)
            DynamicFall();
        if (jumping)
        {
            Jump();
            jumping = false;
        }
        Move(movementX, movementY);
    }

    protected virtual void Update()
    {
        if (!dead)
        {
            ControlCoyote();
            CheckStunLock();
            CheckFatigue();
            CheckDodge();
            CheckParry();
            if (InputStatus == Enum_InputStatus.blocked)
            {
                StopMovement(0);
            }
            else
            {
                if (Input.GetAxisRaw(PowerUpButton) != 0 && powerUp != null && powerUp.PowerUpStatus == Enum_PowerUpStatus.available)
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
                    if (Input.GetButtonDown(JumpButton))
                    {
                        jumping = true;
                    }

                    if (InputStatus != Enum_InputStatus.blocked)
                    {
                        movementX = Input.GetAxisRaw(HorizontalCtrl);
                        if (!IsGrounded() && Input.GetAxis(VerticalCtrl) == -1)
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
        if (rb != null && rb.velocity.y < jumpVelocityAtApex && !IsGrounded() /*&& coyoteFrameCounter > coyoteTimeFrames*/ && dodgeStatus == Enum_DodgeStatus.ready && attacking == false && rb.gravityScale == 1.0f)
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

    protected virtual void IncreaseLimitBreakPerSecond()
    {
        limitBreakGauge = Mathf.Min(limitBreakGauge + limitBreakPerSecond * Time.deltaTime, maxLimitBreakGauge);
    }

    protected virtual void PrimaryAttack()
    {
        if(attackToken > 0)
        {
            /*
             * ORIGINAL
             */

            //animator.SetTrigger("PrimaryAttack");

            /*
             *  REFACTORING
             */
            combo1.StartAttack();

            /*
             * END
             */

            attackToken--;
        }

    }

    protected virtual void SecondaryAttack()
    {
        animator.SetTrigger("SecondaryAttack");
        ReduceStamina(secondaryFireStaminaCost);
        inputStatus = Enum_InputStatus.blocked;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0.0f;
    }

    protected abstract void CastHitBox(int attackType);

    /*
     * ORIGINAL
     */

    /*
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
    */

    /*
     * REFACTORING
     */

    protected void StartAttackString()
    {
        ReduceStamina(combo1.staminaCost);
        inputStatus = Enum_InputStatus.onlyAttack;
        movementX = 0;
        movementY = 0;
        attacking = true;
        rb.velocity = Vector2.zero;
    }

    protected virtual void MoveOnAttack()
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
    /*
     * END
     */

    protected virtual void EndAttackString()
    {
        inputStatus = Enum_InputStatus.allowed;
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

    public virtual void ConsumeStamina(float amount) //DOESN'T TRIGGER THE STAMINA BLOCKED TIMER
    {
        if (amount != 0.0f)
        {
            stamina = Mathf.Max(stamina - amount, 0);
            CheckFatigue();
        }
    }
    public void ApplyStunLock(int duration) // Player can't execute action while damaged
    {
        rb.gravityScale = 1.0f;
        stunlockFrameCounter = 0;
        framesToStunLock = duration;
        guardStatus = Enum_GuardStatus.noGuard;
        inputStatus = Enum_InputStatus.blocked;
        animator.SetBool("Damaged", true);
        animator.SetBool("Guarding", false);
    }

    public void CheckStunLock()
    {
        if (framesToStunLock > 0)
        {
            stunlockFrameCounter++;
            if (stunlockFrameCounter >= framesToStunLock)
            {
                framesToStunLock = 0;
                stunlockFrameCounter = 0;
                rb.velocity = new Vector2(0, rb.velocity.y);
                attacking = false;
                animator.SetBool("Damaged", false);
                inputStatus = Enum_InputStatus.allowed;
            }
        }
    }

    public void ApplyDamage(int dmg, float attackerFacing, int stunLock, Vector2 recoilForce, bool guardBreaker = false)
    {
        if (!Immunity)
        {
            if (guardStatus == Enum_GuardStatus.guarding) 
            {
                if (!guardBreaker && attackerFacing != facing) // attacker is in front of the player and player is guarding, the attacker isn't guard breaking
                {
                    ReduceStamina(dmg * blockStaminaCostMultiplier);
                    dmg = (int)Mathf.Ceil(dmg * damageReductionMultiplier);
                    animator.SetTrigger("Blocked");
                    ResetAttackTokens();
                }
                else //the attack is coming from behind or the attack is a guard breaker
                {
                    animator.SetFloat("AttackerFacing", attackerFacing);
                    ApplyStunLock(stunLock);
                    rb.AddForce(recoilForce * attackerFacing, ForceMode2D.Impulse);
                    ResetAttackTokens();
                }
                health = Mathf.Max(health - dmg, 0);
                Debug.Log("Health :" + health);
                if (cameraController != null)
                {
                    cameraController.Shake(dmg, 5, 1000);
                }
            }
            else //attacker is behind the player or the player is not guarding
            {
                if (!guardBreaker) //if the attack isn't a guard break
                {
                    animator.SetFloat("AttackerFacing", attackerFacing);
                    ApplyStunLock(stunLock);
                    rb.AddForce(recoilForce * attackerFacing, ForceMode2D.Impulse);
                    health = Mathf.Max(health - dmg, 0);
                    Debug.Log("Health :" + health);
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
                Debug.Log("Parried");
            }
        }

        


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
        float currentSpeed = Fatigue ? speed * fatiguedSpeedReduction : speed;
        if(specialStatus == Enum_SpecialStatus.slow)
            currentSpeed *= slowSpeedReduction;
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
    public void ResetAttackTokens()
    {
        attackToken = maxAttackToken;
    }

    /*
     * ORIGINAL
     */

    /*
    protected void DealDamageToEnemies(Collider2D[] enemies, int damage, int stunLock, Vector2 recoilForce, bool specialEffect = false)
    {
        if (enemies.Length > 0)
            foreach (Collider2D enemy in enemies)
            {
                if (enemy.gameObject.tag.Equals("Breakable"))
                {
                    BreakableLife breakableLife = enemy.gameObject.GetComponent<BreakableLife>();
                    breakableLife.TakeDamage(1);
                }
                else
                {
                    DealDamageToEnemy(enemy, damage, stunLock, recoilForce, specialEffect);
                }
            }
    }
    protected void DealDamageToEnemy(Collider2D enemy, int damage, int stunLock, Vector2 recoilForce, bool specialEffect)
    {
        Champion foe = enemy.gameObject.GetComponent<Champion>();
        if (foe != null && foe != this && !foe.Dead)
        {
            Debug.Log("Hit " + foe.transform.parent.name);
            foe.ApplyDamage(damage, facing, stunLock, recoilForce);
            ApplySpecialEffect(foe);
        }
    }*/

    /*
     * REFACTORING
     */
    protected void DealDamageToEnemies(Collider2D[] enemies, int damage, int stunLock, Vector2 recoilForce, bool specialEffect = false)
    {

    }

    /*
    * END
    */

    protected virtual void ApplySpecialEffect(Champion enemy)
    {
        Debug.Log("No special effect on this attack");
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
    public virtual bool IsGrounded()
    {
        //returns true if collides with an obstacle underneath object
        Vector2 centerOne = new Vector2(physicBox.bounds.center.x - (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y);
        Vector2 centerTwo = new Vector2(physicBox.bounds.center.x + (physicBox.bounds.extents.x / 2) * facing, physicBox.bounds.min.y);
        float radius = 0.1f;

        if (Physics2D.OverlapCircle(centerOne, radius, LayerMask.GetMask("Obstacle")) || Physics2D.OverlapCircle(centerTwo, radius, LayerMask.GetMask("Obstacle")))
        {
            animator.SetBool("Fall", false);
            DisableDiveBox();
            return true;
        }
        return false;
    }

    public void AllowInputs()   //activated in the animation controller
    {
        rb.gravityScale = 1.0f;
        inputStatus = Enum_InputStatus.allowed;
        ResetAttackTokens();
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

    public void UpdateHUD()
    {
        healthSlider.value = health;
        staminaSlider.value = stamina;
        ChangeColorHealthSlider();


        //--- Ci-dessous : A modifier par les vrais valeurs ---------

        ultiImageSlider.fillAmount = 0.75f;

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
		set{ 
			health = value;
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
    public void SetNormalStatus()
    {
        specialStatus = Enum_SpecialStatus.normal;
    }
    public void SetStunStatus()
    {
        specialStatus = Enum_SpecialStatus.stun;
    }
    public void SetPoisonStatus()
    {
        specialStatus = Enum_SpecialStatus.poison;
    }
    public void SetSlowStatus()
    {
        specialStatus = Enum_SpecialStatus.slow;
    }
}
