using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Champion
{

    [Header("Combo2Settings")]
    [SerializeField]
    protected int comboTwoDamage = 20;
    [SerializeField] protected float comboTwoSizeX = 1;
    [SerializeField] protected float comboTwoSizeY = 1;
    [SerializeField] protected float comboTwoOffsetX = 0;
    [SerializeField] protected float comboTwoOffsetY = 0;
    [SerializeField] protected int comboTwoStunLock = 5;
    [SerializeField] protected Vector2 comboTwoRecoilForce;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(comboOneOffsetX, comboOneOffsetY, 0) + transform.position, new Vector3(comboOneSizeX, comboOneSizeY, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(comboTwoOffsetX, comboTwoOffsetY, 0) + transform.position, new Vector3(comboTwoSizeX, comboTwoSizeY, 1));
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
}
