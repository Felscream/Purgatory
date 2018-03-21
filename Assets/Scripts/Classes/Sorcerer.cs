using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Champion
{
    public Attack combo2;

    [Header("UltimateSettings")]
    [SerializeField] private float ultimateMinRadius = 2.0f;
    [SerializeField] private float ultimateMaxRadius = 10.0f;
    [SerializeField] private int ultimateStunLock = 4;
    [SerializeField] private float minToMaxTimeTransition = .75f;
    [SerializeField] private float ultimateDuration = 2.5f;
    [SerializeField] private float ultimateDamage = 12;
    [SerializeField] private float shakeIntensity = 15.0f;
    [SerializeField] private int shakeNumber = 4;
    [SerializeField] private int shakeSpeed = 100;
    
    

    [Header("ProjectileSettings")]
    [SerializeField] protected GameObject manabomb;
    [SerializeField] protected Vector2 projectileSpawnOffset;
    [SerializeField] protected Vector2 altProjectileSpawnOffset;
    [SerializeField] private Vector3 altRotation;
    [SerializeField] private Vector2 altRecoil = new Vector2(-4, 4);

    [Header("SoundSettings")]
    public AudioClip primaryAttackSound;
    public AudioClip specialAttackSound;

    private bool ultimate = false;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(projectileSpawnOffset + (Vector2)transform.position, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(altProjectileSpawnOffset + (Vector2)transform.position, 0.3f);
        //uncomment to teleport, you'll have to comment transform.Translate(Teleportation) in WarpOut()
        //WarpOut();
        Gizmos.DrawWireCube(new Vector3(combo1.offset.x, combo1.offset.y, 0) + transform.position, new Vector3(combo1.size.x, combo1.size.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(combo2.offset.x, combo2.offset.y, 0) + transform.position, new Vector3(combo2.size.x, combo2.size.y, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ultimateMinRadius);
        Gizmos.DrawWireSphere(transform.position, ultimateMaxRadius);
    }
    protected override void Start()
    {
        base.Start();
        combo2.SetUser(this);
        combo2.Clasheable();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();

    }
    protected override void PrimaryAttack()
    {
        base.PrimaryAttack();
    }
    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
    }
    public override void ReduceStamina(float amount)
    {
        if (powerUp is StaminaSaving)
        {
            if (powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
            {
                StaminaSaving temp = (StaminaSaving)powerUp;
                amount *= temp.StaminaCostReductionMultiplier;
            }
        }
        base.ReduceStamina(amount);
    }
    protected override void CastHitBox(int attackType)
    {
        switch (attackType)
        {
            case 0: //combo one
                combo1.CastHitBox();
                break;
            case 1: //combo two
                combo2.CastHitBox();
                break;
            //to implement : special and ultimate
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }

    protected override void CheckDodge()
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
                    rb.velocity = new Vector2(0, 0);
                    //teleportation fired from animation event
                    ReduceStamina(dodgeStaminaCost);
                    dodgeStatus = Enum_DodgeStatus.dodging;
                    inputStatus = Enum_InputStatus.blocked;
                    dodgeToken--;
                    animator.SetBool("Jump", false);
                    animator.SetBool("Fall", false);
                    animator.SetTrigger("WarpOut");
                    immune = true;
                }
                break;
            case Enum_DodgeStatus.dodging:
                dodgeFrameCounter++;
                if (dodgeFrameCounter >= dodgeImmunityEndFrame)
                {
                    immune = false;
                }
                break;
        }
    }

    public void WarpOut()
    {
        float halfColliderWidth = physicBox.bounds.extents.x;
        //World position
        Vector2 wp = transform.position;
        
        RaycastHit2D hit;
        Vector2 destination = new Vector2(wp.x + dodgeSpeed * facing, wp.y);
        float raycastLength = dodgeSpeed + halfColliderWidth;
        hit = Physics2D.Raycast(wp, Vector2.right * facing, raycastLength, LayerMask.GetMask("Obstacle"));
        if(hit.collider != null)
        {
            destination = new Vector2(hit.point.x - halfColliderWidth * facing, hit.point.y);
            
            //for debugging purposes
            /*Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.3f);*/
        }

        //for debugging purposes
        /*Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(destination.x, destination.y, 0), 0.5f);*/

        Vector2 teleportation = new Vector2(destination.x - wp.x, 0);
        transform.Translate(teleportation);
    }

    public void WarpIn()
    {
        immune = false;
        rb.velocity = new Vector2(0, 0);
        animator.SetBool("Dodge", false);
        dodgeStatus = Enum_DodgeStatus.ready;
        inputStatus = Enum_InputStatus.allowed;
    }

    public void SpawnManaBomb()
    {
        Vector2 SpawnPoint = new Vector2(transform.position.x + projectileSpawnOffset.x * facing, transform.position.y + projectileSpawnOffset.y);
        GameObject bomb = Instantiate(manabomb, SpawnPoint, transform.rotation);
        Manabomb mb = bomb.GetComponent<Manabomb>();
        mb.Owner = this;
        mb.Direction = facing;
        mb.GetComponent<Rigidbody2D>().AddForce(mb.Force * facing);
        rb.gravityScale = 1.0f;
        AllowInputs();
    }

    public void SpawnAltManaBomb()
    {
        Vector2 SpawnPoint = new Vector2(transform.position.x + altProjectileSpawnOffset.x * facing, transform.position.y + altProjectileSpawnOffset.y);
        GameObject bomb = Instantiate(manabomb, SpawnPoint, Quaternion.Euler(altRotation * facing));
        Manabomb mb = bomb.GetComponent<Manabomb>();
        mb.Owner = this;
        mb.Direction = facing;
        Vector2 alForce = new Vector2(mb.AltForce.x * facing, mb.AltForce.y);
        mb.GetComponent<Rigidbody2D>().AddForce(alForce);
        AllowInputs();
    }

    public void AltManaBombRecoil()
    {
        rb.AddForce(new Vector2(altRecoil.x * facing, altRecoil.y), ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;
    }
    protected override void Ultimate()
    {
        ParticleSystem.EmissionModule temp = ultimateParticleSystem.emission;
        temp.enabled = true;
        inputStatus = Enum_InputStatus.blocked;
        animator.SetBool("Ultimate", true);
        rb.velocity = Vector2.zero;
        //rb.isKinematic = true;
        rb.gravityScale = 0.0f;
        immune = true;
        
    }

    public void CastBarrier()
    {
        if (!ultimate)
        {
            animator.SetBool("Barrier", true);
            animator.SetBool("Ultimate", false);
            StartCoroutine(CastBarrierCoroutine());
        }
        
    }
    private IEnumerator CastBarrierCoroutine()
    {
        ultimate = true;
        
        float timer = 0.0f;
        float radius = ultimateMinRadius;
        float difference = (ultimateMaxRadius - ultimateMinRadius) / minToMaxTimeTransition;
        while (timer <= ultimateDuration)
        {
            InvincibilityVisualizer();
            radius = Mathf.Min(radius + difference * Time.deltaTime, ultimateMaxRadius);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, hitBoxLayer);
            UltimateHits(hits);
            cameraController.Shake(shakeIntensity, shakeNumber, shakeSpeed, true);
            timer += Time.deltaTime;
            yield return null;
        }
        ParticleSystem.EmissionModule temp = ultimateParticleSystem.emission;
        temp.enabled = false;
        immune = false;
        InvincibilityVisualizer();
        rb.isKinematic = false;
        rb.gravityScale = 1.0f;
        animator.SetBool("Barrier", false);
        if (!IsGrounded())
        {
            DynamicFall();
        }
        AllowInputs();
        EndAttackString();
        ResetLimitBreak();
        
        ultimate = false;
    }

    private void UltimateHits(Collider2D[] hits)
    {
        float damage = ultimateDamage * Time.deltaTime;
        foreach(Collider2D col in hits)
        {
            Champion temp = col.GetComponent<Champion>();

            if(temp != null && temp != this && !temp.Dead)
            {
                float direction = Mathf.Sign(col.transform.position.x - transform.position.x);
                temp.ApplyDamage(damage, direction, ultimateStunLock, new Vector2(0, 0), false, true, this);
                temp.RB.velocity = Vector2.zero;
            }
        }
    }

    public void PrimaryAttackSound()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(primaryAttackSound, 0.2F);
    }
}
