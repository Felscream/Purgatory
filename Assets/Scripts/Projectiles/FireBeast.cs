using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBeast : Projectile {
    [SerializeField] private float startingSize = 1.0f;
    [SerializeField] private float endSize = 3.0f;
    [SerializeField] private float timeToLive = 1.3f;
    [SerializeField] private ParticleSystem beastParticleSystem;
    [SerializeField] private float shakeIntensity = 15.0f;
    [SerializeField] private int shakeNumber = 4;
    [SerializeField] private int shakeSpeed = 100;
    [SerializeField] private Vector2 projectionForce;
    private float speed = 0.0f;
    public ParticleSystemRenderer beastRenderer;
    public Material rightMaterial;
    public Material leftMaterial;

    
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void HandleImpact(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Champion>() != null)
        {
            Debug.Log("hit " + collision.gameObject.name);
        }
        else if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            Debug.Log("hit " + collision.gameObject.name);
        }
        else
        {
            Debug.Log("Hit obstacle");
        }
    }



    public IEnumerator StopCollisionDetection()
    {
        ParticleSystem.MainModule main = beastParticleSystem.main;
        main.duration = timeToLive;
        main.startLifetime = timeToLive;
        StartCoroutine(CameraShakes());
        yield return new WaitForSeconds(timeToLive);
        DestroyProjectile();
    }

    private IEnumerator CameraShakes()
    {
        float timer = 0.0f;
        while(timer < timeToLive * 0.75f)
        {
            Camera.main.GetComponent<CameraControl>().Shake(shakeIntensity, shakeNumber, shakeSpeed, true);
            timer += Time.deltaTime;
            yield return null;
        }
        
    }

    public override void DestroyProjectile()
    {
        ParticleSystem.EmissionModule temp = ps.emission;
        temp.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        Destroy(gameObject, timeToDestroy);
    }

    protected override void SetImpact()
    {
        Destroy(gameObject);
    }

    private void HandleTrigger(Collider2D collider)
    {
        Champion foe = collider.gameObject.GetComponent<Champion>();
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
                    foe.ApplyDamage(damage, direction, stunLock, recoilForce, false, true, owner);
                    Debug.Log(direction);
                    foe.SetProjectedStatus(direction, projectionForce, 2);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTrigger(collision);
    }

    public ParticleSystem BeastParticleSystem
    {
        get
        {
            return beastParticleSystem;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            Speed = value;
        }
    }

    public float TimeToLive
    {
        get
        {
            return timeToLive;
        }
        set
        {
            timeToLive = value;
        }
    }

   
}
