using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Champion
{

    [Header("Combo2Settings")]
    [SerializeField]
    protected int comboTwoDamage = 20;
    [SerializeField] protected Vector2 comboTwoOffset = new Vector2(0, 0);
    [SerializeField] protected Vector2 comboTwoSize = new Vector2(1, 1);
    [SerializeField] protected int comboTwoStunLock = 5;
    [SerializeField] protected Vector2 comboTwoRecoilForce;

    [Header("ProjectileSetting")]
    [SerializeField] protected GameObject manabomb;
    [SerializeField] protected Vector2 projectileSpawnOffset;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(comboOneOffset.x, comboOneOffset.y, 0) + transform.position, new Vector3(comboOneSize.x, comboOneSize.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(comboTwoOffset.x, comboTwoOffset.y, 0) + transform.position, new Vector3(comboTwoSize.x, comboTwoSize.y, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(projectileSpawnOffset + (Vector2)transform.position, 0.3f);
        //uncomment to teleportation, you'll have to comment transform.Translate(Teleportation) in WarpOut()
        //WarpOut();

        
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
        inputStatus = Enum_InputStatus.blocked;
    }
    public override void ReduceStamina(float amount)
    {
        if (amount != 0.0f)
        {
            if (powerUp is StaminaSaving)
            {
                if (powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
                {
                    StaminaSaving temp = (StaminaSaving)powerUp;
                    amount *= temp.StaminaCostReductionMultiplier;
                }
            }
            stamina = Mathf.Max(stamina - amount, 0);
            CheckFatigue();
            staminaRegenerationStatus = Enum_StaminaRegeneration.blocked;
            staminablockedTimer = 0.0f;
        }
    }
    protected override void CastHitBox(int attackType)
    {
        Vector2 pos = new Vector2(0, 0);
        Vector2 size = new Vector2(0, 0);
        Vector2 recoilForce = Vector2.zero;
        int damage = 0;
        int stunLock = 0;
        float dir = facing != 0.0f ? facing : 1;
        Collider2D[] hits;
        if (attackType >= 0 && attackType < 5)
        {
            switch (attackType)
            {
                case 0: //combo one
                    pos = new Vector2(comboOneOffset.x * dir, comboOneOffset.y) + (Vector2)transform.position;
                    size = comboOneSize;
                    damage = comboOneDamage;
                    stunLock = comboOneStunLock;
                    recoilForce = comboOneRecoilForce;
                    break;
                case 1: //combo two
                    pos = new Vector2(comboTwoOffset.x * dir, comboTwoOffset.y) + (Vector2)transform.position;
                    size = comboTwoSize;
                    damage = comboTwoDamage;
                    stunLock = comboTwoStunLock;
                    recoilForce = comboTwoRecoilForce;
                    break;
                //to implement : special and ultimate
                default:
                    Debug.LogError("Unknown AttackType");
                    break;
            }
        }
        hits = Physics2D.OverlapBoxAll(pos, size, Vector2.Angle(Vector2.zero, transform.position), hitBoxLayer);
        DealDamageToEnnemies(hits, damage, stunLock, recoilForce);
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
}
