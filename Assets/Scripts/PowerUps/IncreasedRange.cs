using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedRange : PowerUp
{

    [SerializeField] private float timeToDisable = 1.5f;
    [SerializeField] private float staminaCostPerSecond = 10.0f;
    private float disableTimer = 0.0f;

    protected override void LateUpdate()
    {
        if (holder.Fatigue)
        {
            powerUpStatus = Enum_PowerUpStatus.onCooldown;
        }
        base.LateUpdate();
        if(powerUpStatus == Enum_PowerUpStatus.activated)
        {
            disableTimer += Time.deltaTime;
            holder.ConsumeStamina(staminaCostPerSecond * Time.deltaTime);
        }
        else
        {
            disableTimer = 0.0f;
        }
        
    }

    public void StopPowerUp()
    {
        if(disableTimer >= timeToDisable)
        {
            cooldownTimer = 0.0f;
            powerUpStatus = Enum_PowerUpStatus.onCooldown;
            anim.SetBool("PoweredUp", false);
            
        }
    }
    
}
