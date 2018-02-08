using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Champion {

    [Header("Combo2Settings")]
    [SerializeField] protected int comboTwoDamage = 20;
    [SerializeField] protected float comboTwoSizeX = 1;
    [SerializeField] protected float comboTwoSizeY = 1;
    [SerializeField] protected float comboTwoOffsetX = 0;
    [SerializeField] protected float comboTwoOffsetY = 0;

    [Header("EnhancedCombo1Settings")]
    [SerializeField] protected float EnhancedComboOneSizeX = 1;
    [SerializeField] protected float EnhancedComboOneSizeY = 1;
    [SerializeField] protected float EnhancedComboOneOffsetX = 0;
    [SerializeField] protected float EnhancedComboOneOffsetY = 0;

    [Header("EnhancedCombo2Settings")]
    [SerializeField] protected float EnhancedComboTwoSizeX = 1;
    [SerializeField] protected float EnhancedComboTwoSizeY = 1;
    [SerializeField] protected float EnhancedComboTwoOffsetX = 0;
    [SerializeField] protected float EnhancedComboTwoOffsetY = 0;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(comboOneOffsetX, comboOneOffsetY, 0) + transform.position, new Vector3(comboOneSizeX, comboOneSizeY, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(comboTwoOffsetX, comboTwoOffsetY, 0) + transform.position, new Vector3(comboTwoSizeX, comboTwoSizeY, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(EnhancedComboOneOffsetX, EnhancedComboOneOffsetY, 0) + transform.position, new Vector3(EnhancedComboOneSizeX, EnhancedComboOneSizeY, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(EnhancedComboTwoOffsetX, EnhancedComboTwoOffsetY, 1) + transform.position, new Vector3(EnhancedComboTwoSizeX, EnhancedComboTwoSizeY, 1));
    }

    protected override void Update()
    {
        base.Update();
        if(Input.GetAxisRaw(PowerUpButton) != 0 && powerUp.PowerUpStatus == Enum_PowerUpStatus.activated)
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
        int damage = 0;
        float dir = facing != 0.0f ? facing : 1;
        Collider2D[] hits;
        if(attackType >= 0 && attackType < 5)
        {
            switch (attackType)
            {
                case 0: //combo one
                    Debug.Log("1");
                    pos = new Vector2(comboOneOffsetX * dir, comboOneOffsetY) + (Vector2)transform.position;
                    size = new Vector2(comboOneSizeX, comboOneSizeY);
                    damage = comboOneDamage;
                    //hits = Physics2D.OverlapBoxAll(pos, size, 0.0f, LayerMask.NameToLayer("Obstacle"));
                    break;
                case 1: //combo two
                    pos = new Vector2(comboTwoOffsetX * dir, comboTwoOffsetY) + (Vector2)transform.position;
                    size = new Vector2(comboTwoSizeX, comboTwoSizeY);
                    damage = comboTwoDamage;
                    
                    break;
                case 2: //enhanced combo one
                    pos = new Vector2(EnhancedComboOneOffsetX * dir, EnhancedComboOneOffsetY) + (Vector2)transform.position;
                    size = new Vector2(EnhancedComboOneSizeX, EnhancedComboOneSizeY);
                    damage = comboOneDamage;
                    //hits = Physics2D.OverlapBoxAll(pos, size, 0.0f, hitBoxLayer);
                    break;
                case 3: // enhanced combo two
                    pos = new Vector2(EnhancedComboTwoOffsetX * dir, EnhancedComboTwoOffsetY) + (Vector2)transform.position;
                    size = new Vector2(EnhancedComboTwoSizeX, EnhancedComboTwoSizeY);
                    damage = comboTwoDamage;
                    //hits = Physics2D.OverlapBoxAll(pos, size, 0.0f, LayerMask.NameToLayer("Obstacle"));
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
                if(col.gameObject.transform.parent.name != transform.parent.name)
                {
                    Debug.Log("Hit " + col.gameObject.name);
                    col.gameObject.GetComponent<Champion>().ApplyDamage(damage);
                }
            }
        }
    }
}
