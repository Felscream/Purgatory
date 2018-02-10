using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Champion {

    [Header("Combo2Settings")]
    [SerializeField] protected int comboTwoDamage = 20;
    [SerializeField] protected Vector2 comboTwoOffset = new Vector2(0, 0);
    [SerializeField] protected Vector2 comboTwoSize = new Vector2(1, 1);
    [SerializeField] protected int comboTwoStunLock = 5;
    [SerializeField] protected Vector2 comboTwoRecoilForce;

    [Header("EnhancedCombo1Settings")]
    [SerializeField] protected Vector2 EnhancedComboOneOffset = new Vector2(0, 0);
    [SerializeField] protected Vector2 EnhancedComboOneSize = new Vector2(1, 1);
    [SerializeField] protected int EnhancedComboOneStunLock = 5;
    [SerializeField] protected Vector2 EnhancedComboOneRecoilForce;

    [Header("EnhancedCombo2Settings")]
    [SerializeField] protected Vector2 EnhancedComboTwoOffset = new Vector2(0, 0);
    [SerializeField] protected Vector2 EnhancedComboTwoSize = new Vector2(1, 1);
    [SerializeField] protected int EnhancedComboTwoStunLock = 5;
    [SerializeField] protected Vector2 EnhancedComboTwoRecoilForce;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(comboOneOffset.x, comboOneOffset.y, 0) + transform.position, new Vector3(comboOneSize.x, comboOneSize.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(comboTwoOffset.x, comboTwoOffset.y, 0) + transform.position, new Vector3(comboTwoSize.x, comboTwoSize.y, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(EnhancedComboOneOffset.x, EnhancedComboOneOffset.y, 0) + transform.position, new Vector3(EnhancedComboOneSize.x, EnhancedComboOneSize.y, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(EnhancedComboTwoOffset.x, EnhancedComboTwoOffset.y, 1) + transform.position, new Vector3(EnhancedComboTwoSize.x, EnhancedComboTwoSize.y, 1));
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
        throw new System.NotImplementedException();
    }

    protected override void CastHitBox(int attackType)
    {
        Debug.Log("In");
        Vector2 pos = new Vector2(0,0);
        Vector2 size = new Vector2(0, 0);
        Vector2 recoilForce = Vector2.zero;
        int damage = 0;
        int stunLock = 0;
        float dir = facing != 0.0f ? facing : 1;
        Collider2D[] hits;
        if(attackType >= 0 && attackType < 5)
        {
            switch (attackType)
            {
                case 0: //combo one
                    Debug.Log("1");
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
                case 2: //enhanced combo one
                    pos = new Vector2(EnhancedComboOneOffset.x * dir, EnhancedComboOneOffset.y) + (Vector2)transform.position;
                    size = new Vector2(EnhancedComboOneSize.x, EnhancedComboOneSize.y);
                    damage = comboOneDamage;
                    stunLock = EnhancedComboOneStunLock;
                    recoilForce = EnhancedComboOneRecoilForce;
                    break;
                case 3: // enhanced combo two
                    pos = new Vector2(EnhancedComboTwoOffset.x * dir, EnhancedComboTwoOffset.y) + (Vector2)transform.position;
                    size = new Vector2(EnhancedComboTwoSize.x, EnhancedComboTwoSize.y);
                    damage = comboTwoDamage;
                    stunLock = EnhancedComboTwoStunLock;
                    recoilForce = EnhancedComboTwoRecoilForce;
                    break;
                //to implement : special and ultimate
                default:
                    Debug.LogError("Unknown AttackType");
                    break;
            }
        }
        hits = Physics2D.OverlapBoxAll(pos, size, Vector2.Angle(Vector2.zero, transform.position), hitBoxLayer);
        if (hits.Length > 0)
        {
            foreach(Collider2D col in hits)
            {
                if(col.gameObject.transform.parent.name != transform.parent.name && !col.gameObject.GetComponent<Champion>().Dead)
                {
                    Debug.Log("Hit " + col.gameObject.name);
                    col.gameObject.GetComponent<Champion>().ApplyDamage(damage, facing, stunLock, recoilForce);
                }
            }
        }
    }
}
