using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;


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

    [Header("HUD Settings")]
    [Range(0, 1)] public float transparencyOnCooldown = 0.4f;
    [Range(0, 1)] public float transparencyAvailable = 1.0f;

    protected Champion holder;
    protected CanvasGroup canvasgrp;
    protected Animator anim;
    protected float cooldownTimer = 0.0f, activationTimer = 0.0f;
    protected Enum_PowerUpStatus powerUpStatus;

    protected Image powerUpImageSlider;
    protected Image powerUpAbilityImage;
    // Use this for initialization
    protected virtual void Start () {
        holder = GetComponent<Champion>();
        
        if (holder != null)
        {
            powerUpStatus = Enum_PowerUpStatus.available;
            canvasgrp = holder.playerHUD;
            powerUpImageSlider = canvasgrp.transform.Find("PowerUpImage").Find("RadialSliderImage").GetComponent<Image>();
            powerUpImageSlider.fillAmount = 1.0f;
            powerUpAbilityImage = canvasgrp.transform.Find("PowerUpImage").Find("AbilityImage1").GetComponent<Image>();
        }
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	 protected virtual void LateUpdate () {
        switch (powerUpStatus)
        {
            case Enum_PowerUpStatus.available:
                powerUpImageSlider.fillAmount = 1.0f;
                powerUpAbilityImage.color = new Color(255, 255, 255, transparencyAvailable);
                powerUpAbilityImage.transform.parent.localScale = new Vector2(1.0f, 1.0f);
                break;
            case Enum_PowerUpStatus.activated:

                powerUpAbilityImage.transform.parent.localScale = new Vector2(1.25f, 1.25f);
                powerUpAbilityImage.color = new Color(255, 255, 255, transparencyAvailable);
                CheckActivationDuration();
                break;
            case Enum_PowerUpStatus.onCooldown:
                powerUpAbilityImage.transform.parent.localScale = new Vector2(1.0f, 1.0f);
                powerUpAbilityImage.color = new Color(255, 255, 255, transparencyOnCooldown);
                CheckCooldownDuration();
                break;
        }

        
    }

    public virtual void ActivatePowerUp()
    {
        if(powerUpStatus == Enum_PowerUpStatus.available)
        {
            Debug.Log("PoweredUp");
            powerUpStatus = Enum_PowerUpStatus.activated;
            activationTimer = 0.0f;
            anim.SetBool("PoweredUp", true);
        }
            
    }

    protected virtual void CheckActivationDuration()
    {
        if(duration != 0)
        {
            activationTimer += Time.deltaTime;
            powerUpImageSlider.fillAmount = 1 - activationTimer / duration;
            if (activationTimer >= duration)
            {
                powerUpStatus = Enum_PowerUpStatus.onCooldown;
                cooldownTimer = 0.0f;
                anim.SetBool("PoweredUp", false);
            }
        }
        
    }
    
    protected virtual void CheckCooldownDuration()
    {
        if(cooldown != 0)
        {
            anim.SetBool("PoweredUp", false);
            cooldownTimer += Time.deltaTime;
            powerUpImageSlider.fillAmount = cooldownTimer / cooldown;
            if (cooldownTimer >= cooldown)
            {
                powerUpStatus = Enum_PowerUpStatus.available;
            }
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
