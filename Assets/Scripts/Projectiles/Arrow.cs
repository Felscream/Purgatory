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
    [SerializeField] protected float poisonDamage = 2.0f;
    [Header("SoundSettings")]
    public AudioClip arrowSound;
    AudioSource audioSource;
    protected ParticleSystem.ColorOverLifetimeModule particleColor;
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
            GameObject stunAnim = Instantiate(stunSprite, foe.transform.position, foe.transform.rotation);
            stunAnim.transform.SetParent(foe.transform);
            stunAnim.transform.Translate(new Vector3(0.0f, 1.25f, 0.0f));
            stunAnim.transform.localScale += new Vector3(1.5F, 1.5F, 0);
            Destroy(stunAnim, 5.0f);
            foe.SetStunStatus();
        }
        else
        {
            if (arrowStatus == Enum_SpecialStatus.poison)
            {
                GameObject stunAnim = Instantiate(poisonSprite, foe.transform.position, foe.transform.rotation);
                stunAnim.transform.SetParent(foe.transform);
                stunAnim.transform.Translate(new Vector3(0.0f, 0.0f, -0.1f));
                stunAnim.transform.localScale += new Vector3(1.25F, 1.3F, 0);
                Destroy(stunAnim, 5f);
                foe.SetPoisonStatus(PoisonDamage);
            }
            else
            {
                if (arrowStatus == Enum_SpecialStatus.slow)
                {
                    GameObject slowAnim = Instantiate(slowSprite, foe.transform.position, foe.transform.rotation);
                    slowAnim.transform.SetParent(foe.transform);
                    slowAnim.transform.Translate(new Vector3(0.2f, 0.0f, -0.1f));
                    slowAnim.transform.localScale += new Vector3(1.0F, 1.5F, 0);
                    Destroy(slowAnim, 5f);
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
    public void ArrowSound()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(arrowSound, 1.0F);
    }
}
