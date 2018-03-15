﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {

    [SerializeField] protected Vector2 force;
    [SerializeField] protected float maxTravelDistance;
    [SerializeField] protected int damage;
    [SerializeField] protected int stunLock;
    [SerializeField] protected Vector2 recoilForce;

    protected float direction = 1.0f, distanceTraveled;
    protected Animator anim;
    protected Champion owner;
    protected List<Champion> hits = new List<Champion>();
    protected Vector2 translation;
    protected bool impact = false;
    protected Vector3 lastPosition;
    protected Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        distanceTraveled = 0.0f;
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Direction", direction);
        }
        //translation = new Vector2(distanceUnitPerSeconds * direction, 0) * Time.deltaTime;
        lastPosition = transform.position;
        hits.Clear();
        rb = GetComponent<Rigidbody2D>();
    }
	
    protected virtual void FixedUpdate()
    {
        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        if (maxTravelDistance != 0)
        {
            distanceTraveled += Vector3.Distance(lastPosition, transform.position);
            lastPosition = transform.position;
            if (distanceTraveled >= maxTravelDistance)
            {
                SetImpact();
            }
        }
    }

    protected virtual void HandleImpact(Collision2D collision)
    {
        Champion foe = collision.gameObject.GetComponent<Champion>();
        if (foe != null) //a player has been hit
        {
            if(foe != owner && !foe.Dead)   //the player struck is not the owner of the projectile and is not dead
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
                    SetImpact();
                }
            }
        }
        else //we hit a wall or another projectile
        {
            SetImpact();
        }
    }

    protected virtual void SetImpact()
    {
        anim.SetTrigger("Impact");
        rb.velocity = Vector2.zero;
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact(collision);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        HandleImpact(collision);
    }

    public virtual void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
    
    public float Direction
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;
        }
    }

    public Champion Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }

    public Vector2 Force
    {
        get
        {
            return force;
        }
    }
}
