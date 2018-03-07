using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack {

    public Vector2 offset = new Vector2(0, 0);
    public Vector2 size = new Vector2(1, 1);
    public int stunLock = 5;
    public Vector2 recoilForce;
    public string animatorTrigger;
    public float staminaCost;
    public int damage = 10;
    public float movementForce = 2;
    public bool haveSpecialEffect = false;
    public Enum_SpecialStatus specialEffect;

    protected Animator animator;
    protected Rigidbody2D rb;
    protected Champion user;
    protected LayerMask hitBoxLayer;

    protected float face;
    
    public void SetUser(Champion c)
    {
        user = c;
        animator = user.Animator;
        rb = user.RB;
        hitBoxLayer = user.HitBoxLayer;
    }

    public void StartAttack()
    {
        animator.SetTrigger(animatorTrigger);
        //rb.velocity = Vector2.zero;
    }

    public void MoveOnAttack()
    {
        Vector2 force = new Vector2(user.Facing * movementForce, 0);
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void CastHitBox()
    {
        Collider2D[] hits;
        Vector2 pos = new Vector2(0, 0);
        float dir = user.Facing != 0.0f ? user.Facing : 1;
        pos = new Vector2(offset.x * dir, offset.y) + user.Position;
        hits = Physics2D.OverlapBoxAll(pos, size, Vector2.Angle(Vector2.zero, user.Position), hitBoxLayer);
        DealDamageToEnemies(hits);
    }

    protected void DealDamageToEnemies(Collider2D[] enemies)
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
                    DealDamageToEnemy(enemy);
                }
            }
    }

    protected void DealDamageToEnemy(Collider2D enemy)
    {
        Champion foe = enemy.gameObject.GetComponent<Champion>();

        if (foe != null && foe != user && !foe.Dead)
        {
            Debug.Log("Hit " + foe.transform.parent.name);
            foe.ApplyDamage(damage, user.Facing, stunLock, recoilForce);
            ApplySpecialEffect(foe);
        }
    }

    protected void ApplySpecialEffect(Champion enemy)
    {
        Debug.Log("No special effect on this attack");
    }


}
