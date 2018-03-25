﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateArrow : Arrow {

    protected override void FixedUpdate()
    {
        direction = Mathf.Sign(rb.velocity.x);
        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        base.FixedUpdate();
    }
    protected override void HandleImpact(Collision2D collision)
    {
        Champion foe = collision.gameObject.GetComponent<Champion>();
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
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
        else
        {
            if(projectile != null)
            {
                DestroyProjectile();
            }
            else
            {

                Debug.Log(rb.velocity);
            }
            
        }
    }
}
