using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Champion
{
    public Attack combo2;
    public Attack enhancedCombo1;
    public Attack enhancedCombo2;
    public Attack ultimate;

    [Header("UltimateProjectileSettings")]
    [SerializeField] private GameObject ultimateProjectile;
    [SerializeField] protected Vector2 projectileSpawnOffset;

    protected bool secondaryAttackRunning = false;
    private bool usePowerUp = false;
    
    public void OnDrawGizmosSelected()
    {
        /*Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(combo1.offset.x, combo1.offset.y, 0) + transform.position, new Vector3(combo1.size.x, combo1.size.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(combo2.offset.x, combo2.offset.y, 0) + transform.position, new Vector3(combo2.size.x, combo2.size.y, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(enhancedCombo1.offset.x, enhancedCombo1.offset.y, 0) + transform.position, new Vector3(enhancedCombo1.size.x, enhancedCombo1.size.y, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(enhancedCombo2.offset.x, enhancedCombo2.offset.y, 1) + transform.position, new Vector3(enhancedCombo2.size.x, enhancedCombo2.size.y, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(specialAttack.offset.x, specialAttack.offset.y, 1) + transform.position, new Vector3(specialAttack.size.x, specialAttack.size.y, 1));*/
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(projectileSpawnOffset + (Vector2)transform.position, 0.3f);
    }

    protected override void Start()
    {
        base.Start();
        combo2.SetUser(this);
        enhancedCombo1.SetUser(this);
        enhancedCombo2.SetUser(this);
        ultimate.SetUser(this);
        combo2.Clasheable();
        enhancedCombo1.Clasheable();
        enhancedCombo2.Clasheable();
    }

    protected override void Update()
    {
        base.Update();
        if(Input.GetAxisRaw(PowerUpButton) != 0 && Input.GetAxis(GuardButton) < 0.6 && powerUp != null && powerUp.PowerUpStatus == Enum_PowerUpStatus.activated && usePowerUp)
        {
            if(powerUp is IncreasedRange)
            {
                IncreasedRange temp = (IncreasedRange)powerUp;
                temp.StopPowerUp();
                usePowerUp = false;
            }
        }
        if (Input.GetAxis(PowerUpButton) == 0)
        {
            usePowerUp = true;
        }
        
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
    
    protected override void CastHitBox(int attackType)
    {
        switch (attackType)
        {
            case 1:
                combo1.CastHitBox();
                break;
            case 2:
                combo2.CastHitBox();
                break;
            case 3:
                enhancedCombo1.CastHitBox();
                break;
            case 4:
                enhancedCombo2.CastHitBox();
                break;
            case 5:
                specialAttack.CastHitBox();
                break;
            case 6:
                ultimate.CastHitBox();
                break;
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }

    public override void MoveOnAttack(int attackID)
    {
        switch (attackID)
        {
            case 1:
                combo1.MoveOnAttack();
                break;
            case 2:
                combo2.MoveOnAttack();
                break;
            case 3:
                enhancedCombo1.MoveOnAttack();
                break;
            case 4:
                enhancedCombo2.MoveOnAttack();
                break;
            case 5:
                specialAttack.MoveOnAttack();
                break;
            case 6:
                ultimate.MoveOnAttack();
                break;
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }

    protected override void Ultimate()
    {
        if (IsGrounded() && rb.velocity.y == 0.0f)
        {
            EndAttackString();
            inputStatus = Enum_InputStatus.blocked;
            animator.SetBool("Ultimate", true);
            rb.velocity = Vector2.zero;
            immune = true;
        }
    }

    public void SpawnFireBeast()
    {
        Vector2 SpawnPoint = new Vector2(transform.position.x + projectileSpawnOffset.x * facing, transform.position.y + projectileSpawnOffset.y);
        GameObject bomb = Instantiate(ultimateProjectile, SpawnPoint, transform.rotation);
        FireBeast fb = bomb.GetComponent<FireBeast>();
        if(facing >= 0)
        {
            fb.beastRenderer.material = fb.rightMaterial;
        }
        else
        {
            fb.beastRenderer.material = fb.leftMaterial;
        }
        fb.Owner = this;
        fb.Direction = facing;
        fb.GetComponent<Rigidbody2D>().AddForce(fb.Force * facing, ForceMode2D.Force);

        float distanceToWall = facing >= 0 ? 20 - transform.position.x : Mathf.Abs(-20 - transform.position.x);
        float timeToLive = distanceToWall / 40;
            
        fb.TimeToLive = timeToLive;
        fb.BeastParticleSystem.Stop();
        StartCoroutine(fb.StopCollisionDetection());
        fb.BeastParticleSystem.Play();
        rb.gravityScale = 1.0f;
        ResetLimitBreak();
    }

    public override void DeathBehaviour()
    {
        animator.SetTrigger("DeadTrigger");
    }

    public void DeadLoop()
    {
        animator.SetBool("Dead", true);
    }
}
