using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    public enum Enum_ArrowStatus
    {
        normal,
        poison,
        stun,
        slow
    }

    public Enum_ArrowStatus arrowStatus = Enum_ArrowStatus.normal;

    [Header("Animations Stu/Poison/Slow")]
    [SerializeField] protected GameObject stunSprite;
    [SerializeField] protected GameObject poisonSprite;
    [SerializeField] protected GameObject slowSprite;

    private bool isStunned = false, isPoisonned = false, isSlowed = false;
    
    void Start()
    {
        // Change color of the arrow
        if (arrowStatus == Enum_ArrowStatus.poison) 
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            if (arrowStatus == Enum_ArrowStatus.stun)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                if (arrowStatus == Enum_ArrowStatus.slow)
                {
                    GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
    }

    protected override void UpdatePosition()
    {
        base.UpdatePosition();
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
                    foe.ApplyDamage(damage, direction, stunLock, recoilForce);
                    ApplyAndShowDebuf(foe);
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
    void ApplyAndShowDebuf(Champion foe)
    {
        if (arrowStatus == Enum_ArrowStatus.stun)
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
            if (arrowStatus == Enum_ArrowStatus.poison)
            {
                GameObject stunAnim = Instantiate(poisonSprite, foe.transform.position, foe.transform.rotation);
                stunAnim.transform.SetParent(foe.transform);
                stunAnim.transform.Translate(new Vector3(0.0f, 0.0f, -0.1f));
                stunAnim.transform.localScale += new Vector3(1.25F, 1.3F, 0);
                Destroy(stunAnim, 5f);
                foe.SetPoisonStatus();
            }
            else
            {
                if (arrowStatus == Enum_ArrowStatus.slow)
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

}
