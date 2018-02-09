using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedRange : PowerUp {

    [SerializeField] private float timeToDisable = 1.5f;
    private float disableTimer = 0.0f;
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if(powerUpStatus == Enum_PowerUpStatus.activated)
        {
            disableTimer += Time.deltaTime;
        }
        else
        {
            disableTimer = 0.0f;
        }
    }
    public void StopPowerUp()
    {
        if(powerUpStatus == Enum_PowerUpStatus.activated && disableTimer >= timeToDisable)
        {
            cooldownTimer = cooldown - (activationTimer / duration) * cooldown;
            powerUpStatus = Enum_PowerUpStatus.onCooldown;
        }
    }
}
