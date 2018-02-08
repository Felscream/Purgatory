using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Enum_PowerUpStatus
{
    available,
    activated,
    onCooldown
}

public abstract class PowerUp : MonoBehaviour {
    [Header("PowerUp Settings")]
    [SerializeField] protected float cooldown = 1;
    [SerializeField] protected float duration = 1;
    [SerializeField] protected float staminaCost = 0.0f;
    protected Champion holder;
    protected Animator anim;
    protected float cooldownTimer, activationTimer;
    protected Enum_PowerUpStatus powerUpStatus;
    // Use this for initialization
    protected void Start () {
        holder = GetComponent<Champion>();
        if(holder != null)
        {
            powerUpStatus = Enum_PowerUpStatus.available;
        }
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	virtual protected void LateUpdate () {
        switch (powerUpStatus)
        {
            case Enum_PowerUpStatus.activated:
                CheckActivationDuration();
                break;
            case Enum_PowerUpStatus.onCooldown:
                CheckCooldownDuration();
                break;
        }
        Debug.Log(powerUpStatus);
	}

    public virtual void ActivatePowerUp()
    {
        
        if(powerUpStatus == Enum_PowerUpStatus.available)
        {
            powerUpStatus = Enum_PowerUpStatus.activated;
            activationTimer = 0.0f;
            anim.SetBool("PoweredUp", true);
        }
            
    }

    protected virtual void CheckActivationDuration()
    {
        activationTimer += Time.deltaTime;
        if (activationTimer >= duration)
        {
            powerUpStatus = Enum_PowerUpStatus.onCooldown;
            cooldownTimer = 0.0f;
            anim.SetBool("PoweredUp", false);
        }
    }
    
    protected virtual void CheckCooldownDuration()
    {
        anim.SetBool("PoweredUp", false);
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= cooldown)
        {
            powerUpStatus = Enum_PowerUpStatus.available;
        }      
    }

    public Enum_PowerUpStatus PowerUpStatus
    {
        get
        {
            return powerUpStatus;
        }
    }

    public float CooldownTimer
    {
        get
        {
            return cooldownTimer;
        }
    }

    public float ActivationTimer
    {
        get
        {
            return activationTimer;
        }
    }
}
