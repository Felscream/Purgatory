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
