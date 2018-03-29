using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack {

    public Vector2 offset = new Vector2(0, 0);
    public Vector2 size = new Vector2(1, 1);
    public int stunLock = 5;
    public Vector2 recoilForce;
    public float projectionDuration = 1.5f;
    public float staminaCost;
    public int damage = 10;
    public float movementForce = 2;
    public bool guardBreaker = false;
    public bool haveSpecialEffect = false;
    public Enum_SpecialStatus specialEffect;

    [SerializeField]protected bool canClash=false;

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
        {
            foreach (Collider2D enemy in enemies)
            {
                if (enemy.gameObject.tag.Equals("Breakable"))
                {
                    BreakableLife breakableLife = enemy.gameObject.GetComponent<BreakableLife>();
                    breakableLife.TakeDamage(1);
                }
                if (enemy.gameObject.tag.Equals("BreakingOrb"))
                {       
                    BreakingOrb breakingOrb = enemy.gameObject.GetComponent<BreakingOrb>();
                    breakingOrb.TakeDamage(1);
                }
                else
                {
                    DealDamageToEnemy(enemy);
                }
            }
        }  
    }

    protected void DealDamageToEnemy(Collider2D enemy)
    {
        Champion foe = enemy.gameObject.GetComponent<Champion>();

        if (foe != null && foe != user && !foe.Dead)
        {
            // Debug.Log("Hit " + foe.transform.parent.name);
            foe.ApplyDamage(damage, user.Facing, stunLock, recoilForce, guardBreaker, canClash, user);
            user.IncreaseLimitBreak(user.LimitBreakOnHit);  //increase limit break
            if (haveSpecialEffect && CanApplySpecialEffect(foe))
            {
                ApplySpecialEffect(foe);
            }
        }
    }
    protected bool CanApplySpecialEffect(Champion foe)
    {
        if (!foe.Immunity)
        {
            if (foe.GuardStatus == Enum_GuardStatus.noGuard)
            {
                return true;
            }
            else
            {
                if (foe.GuardStatus == Enum_GuardStatus.guarding && foe.Facing == user.Facing)
                {
                    return true;
                }
            }
        }
        return false;
    }
    protected void ApplySpecialEffect(Champion enemy)
    {
        switch(specialEffect)
        {
            case Enum_SpecialStatus.stun :
                enemy.SetStunStatus();
                break;
            case Enum_SpecialStatus.poison:
                enemy.SetPoisonStatus();
                break;
            case Enum_SpecialStatus.slow:
                enemy.SetSlowStatus();
                break;
            case Enum_SpecialStatus.projected:
                enemy.SetProjectedStatus(user.Facing, recoilForce, projectionDuration);
                break;
            default:
                Debug.Log("No special effect on this attack");
            break;
        }
        
    }

    public void Clasheable()
    {
        canClash = true;
    }

}
