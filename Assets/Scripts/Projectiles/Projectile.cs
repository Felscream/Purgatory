using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {

    [SerializeField] protected Vector2 force;
    [SerializeField] protected float maxTravelDistance;
    [SerializeField] protected int damage;
    [SerializeField] protected int stunLock;
    [SerializeField] protected Vector2 recoilForce;
    [SerializeField] protected float timeToDestroy = 1.0f;
    [Header("Sound Settings")]
    [SerializeField]
    protected Sound[] soundEffects;


    protected float direction = 1.0f, distanceTraveled;
    protected AudioVolumeManager audioVolumeManager;
    protected Animator anim;
    protected Champion owner;
    protected List<Champion> hits = new List<Champion>();
    protected Vector2 translation;
    protected bool impact = false;
    protected Vector3 lastPosition;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected ParticleSystem ps;
    protected ManagerInGame gameManager;
    // Use this for initialization

    protected virtual void Awake()
    {
        gameManager = ManagerInGame.GetInstance();
        audioVolumeManager = AudioVolumeManager.GetInstance();
        foreach (Sound s in soundEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            gameManager.AddAudioSource(s.source);
            s.source.pitch = s.pitch;
            s.source.volume = audioVolumeManager.SoundEffectVolume;
            s.source.spatialBlend = 0.0f;
        }
        
    }
    protected virtual void Start () {
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
        sr = GetComponent<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        
    }
	
    protected virtual void FixedUpdate()
    {
        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        if (anim != null)
        {
            anim.SetFloat("Direction", direction);
        }
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
    public void PlaySoundEffect(string name)
    {
        Sound s = Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound " + name + " not found");
            return;
        }
        s.source.pitch = s.pitch;
        s.source.volume = audioVolumeManager.SoundEffectVolume;
        s.source.Play();
    }

    public void PlaySoundEffectRandomPitch(string name)
    {
        Sound s = Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound " + name + " not found");
            return;
        }
        float pitch = UnityEngine.Random.Range(0.9f, 1.8f);
        s.source.pitch = pitch;
        s.source.volume = audioVolumeManager.SoundEffectVolume;
        s.source.Play();
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
                    foe.ApplyDamage(damage, direction, stunLock, recoilForce, false, true, owner, true);
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
        
        //anim.SetTrigger("Impact");
        transform.rotation = Quaternion.Euler(0, 0, 0);
        DestroyProjectile();
        
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
        ParticleSystem.EmissionModule temp = ps.emission;
        temp.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        sr.sortingOrder = -3;
        sr.enabled = false;
        PlaySoundEffectRandomPitch("Impact");
        Destroy(gameObject, timeToDestroy);
    }
    protected bool CanApplySpecialEffect(Champion foe)
    {
        if (!foe.Immunity)
        {
            if (foe.GuardStatus == Enum_GuardStatus.noGuard )
            {
                return true;
            }
            else
            {
                if (foe.GuardStatus == Enum_GuardStatus.guarding && foe.Facing == direction)
                {
                    return true;
                }
            }
        }
        return false;
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

    protected void OnDestroy()
    {
        foreach(Sound s in soundEffects)
        {
            gameManager.RemoveAudioSource(s.source);
        }
    }
}
