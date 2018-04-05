using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{

    private Enum_SpecialStatus arrowStatus = Enum_SpecialStatus.normal;

    [Header("Animations Stu/Poison/Slow")]
    [SerializeField] protected GameObject stunSprite;
    [SerializeField] protected GameObject poisonSprite;
    [SerializeField] protected GameObject slowSprite;
    [SerializeField] protected Gradient normal;
    [SerializeField] protected Gradient poison;
    [SerializeField] protected Gradient slow;
    [SerializeField] protected Gradient stun;
    [SerializeField] protected float poisonDamage = 3.0f;
    [SerializeField] protected float poisonDuration = 4.0f;
    AudioSource audioSource;
    protected ParticleSystem.ColorOverLifetimeModule particleColor;
    protected Collider2D arrowCollider;
    protected override void Start()
    {
        base.Start();
        particleColor = ps.colorOverLifetime;
        switch (arrowStatus)
        {
            
            case Enum_SpecialStatus.poison:
                anim.SetTrigger("Poison");
                particleColor.color = poison;
                break;
            case Enum_SpecialStatus.slow:
                anim.SetTrigger("Slow");
                particleColor.color = slow;
                break;
            case Enum_SpecialStatus.stun:
                anim.SetTrigger("Stun");
                particleColor.color = stun;
                break;
            default:
                particleColor.color = normal;
                break;
        }
        arrowCollider = GetComponent<Collider2D>();
    }


    protected override void HandleImpact(Collision2D collision)
    {
        Champion foe = collision.gameObject.GetComponent<Champion>();
        if (foe != null) //a player has been hit
        {
            if (foe != owner && !foe.Dead)   //the player struck is not the owner of the projectile and is not dead
            {
                Champion appearance = null;
                if (hits.Count > 0)         //check if we already hit this player with the same projectile, 
                {
                    foreach (Champion col in hits)
                    {
                        if (foe == col)
                        {
                            appearance = col;
                            break;
                        }
                    }
                }

                if (appearance == null)     //if we didn't, we deal damage
                {
                    hits.Add(foe);
                    foe.ApplyDamage(damage, direction, stunLock, recoilForce, false, true, owner, true);
                    if (CanApplySpecialEffect(foe))
                    {
                        ApplyAndShowDebuf(foe);
                    }
                    
                    //impact = true;
                    DestroyProjectile();
                }
            }
        }
        else //we hit a wall
        {
            DestroyProjectile();
            //impact = true;
        }
    }
    protected void ApplyAndShowDebuf(Champion foe)
    {
        if (arrowStatus == Enum_SpecialStatus.stun)
        {
            foe.SetStunStatus();
        }
        else
        {
            if (arrowStatus == Enum_SpecialStatus.poison)
            {
                foe.SetPoisonStatus(PoisonDamage, poisonDuration);
            }
            else
            {
                if (arrowStatus == Enum_SpecialStatus.slow)
                {
                    foe.SetSlowStatus();
                }
            }
        }   
    }

    public Enum_SpecialStatus ArrowStatus
    {
        get
        {
            return arrowStatus;
        }
        set
        {
            arrowStatus = value;
        }
    }

    public float PoisonDamage
    {
        get
        {
            return poisonDamage;
        }
        set
        {
            poisonDamage = value;
        }
    }

    public Collider2D ArrowCollider
    {
        get
        {
            return arrowCollider;
        }
    }
}
