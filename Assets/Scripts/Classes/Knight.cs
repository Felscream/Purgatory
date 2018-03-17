using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Champion
{
    public Attack combo2;
    public Attack enhancedCombo1;
    public Attack enhancedCombo2;

    protected bool secondaryAttackRunning = false;
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(combo1.offset.x, combo1.offset.y, 0) + transform.position, new Vector3(combo1.size.x, combo1.size.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(combo2.offset.x, combo2.offset.y, 0) + transform.position, new Vector3(combo2.size.x, combo2.size.y, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(enhancedCombo1.offset.x, enhancedCombo1.offset.y, 0) + transform.position, new Vector3(enhancedCombo1.size.x, enhancedCombo1.size.y, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(enhancedCombo2.offset.x, enhancedCombo2.offset.y, 1) + transform.position, new Vector3(enhancedCombo2.size.x, enhancedCombo2.size.y, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(specialAttack.offset.x, specialAttack.offset.y, 1) + transform.position, new Vector3(specialAttack.size.x, specialAttack.size.y, 1));
        
    }

    protected override void Start()
    {
        base.Start();
        combo2.SetUser(this);
        enhancedCombo1.SetUser(this);
        enhancedCombo2.SetUser(this);
        combo2.Clasheable();
        enhancedCombo1.Clasheable();
        enhancedCombo2.Clasheable();
    }

    protected override void Update()
    {
        base.Update();
        if(Input.GetAxisRaw(PowerUpButton) != 0 && powerUp != null && powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
        {
            if(powerUp is IncreasedRange)
            {
                IncreasedRange temp = (IncreasedRange)powerUp;
                temp.StopPowerUp();
            }
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
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }

    protected void MoveOnAttack(int attackID)
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
            default:
                Debug.LogError("Unknown AttackType");
                break;
        }
    }

    protected override void Ultimate()
    {
        throw new System.NotImplementedException();
    }

}
