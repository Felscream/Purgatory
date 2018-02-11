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

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(comboOneOffset.x, comboOneOffset.y, 0) + transform.position, new Vector3(comboOneSize.x, comboOneSize.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(comboTwoOffset.x, comboTwoOffset.y, 0) + transform.position, new Vector3(comboTwoSize.x, comboTwoSize.y, 1));

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
        throw new System.NotImplementedException();
    }

    protected override void CastHitBox(int attackType)
    {
        throw new System.NotImplementedException();
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
            
            /*Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.3f);*/
        }
        
        /*Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(destination.x, destination.y, 0), 0.5f);*/
        Vector2 teleportation = new Vector2(destination.x - wp.x, 0);
        Debug.Log(destination + " " + wp + " " + teleportation);
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
}
