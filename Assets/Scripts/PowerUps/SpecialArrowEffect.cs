using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialArrowEffect : PowerUp
{
    /*[SerializeField] private bool isNormal = true;
    [SerializeField] private bool isStunned = false;
    [SerializeField] private bool isPoisonned = false;
    [SerializeField] private bool isSlowed = false;*/

    [SerializeField] private int maxStacks = 3;
    private Enum_SpecialStatus statusEffect;
    private bool reloading = false;
    private Coroutine reloadStackCoroutine;
    private int currentStacks;

    protected override void Start()
    {
        base.Start();
        currentStacks = maxStacks;
    }
    public override void ActivatePowerUp()
    {
        if (powerUpStatus == Enum_PowerUpStatus.available && currentStacks > 0)
        {
            powerUpStatus = Enum_PowerUpStatus.activated;
            GetRandomPowerUp();
            if (reloadStackCoroutine != null)
            {
                StopCoroutine(reloadStackCoroutine); //les stacks ne se rechargent pas si le power up est actif
                reloading = false;
                powerUpImageSlider.fillAmount = 0.0f;
            }
            anim.SetBool("PoweredUp", true);
        }
    }

    protected override void LateUpdate()
    {
        if(currentStacks != maxStacks) 
        {
            if (!reloading && powerUpStatus != Enum_PowerUpStatus.activated) //les stacks ne se recharge pas si le power up est actif
            {
                reloadStackCoroutine = StartCoroutine(ReloadStack());
            }
        }
        switch (powerUpStatus)
        {
            case Enum_PowerUpStatus.available:
                //powerUpImageSlider.fillAmount = 1.0f;
                powerUpAbilityImage.color = new Color(255, 255, 255, transparencyAvailable);
                powerUpAbilityImage.transform.parent.localScale = new Vector2(1.0f, 1.0f);
                break;
            case Enum_PowerUpStatus.activated:
                powerUpAbilityImage.transform.parent.localScale = new Vector2(1.25f, 1.25f);
                powerUpAbilityImage.color = new Color(255, 255, 255, transparencyAvailable);
                break;
            case Enum_PowerUpStatus.onCooldown:
                powerUpAbilityImage.transform.parent.localScale = new Vector2(1.0f, 1.0f);
                powerUpAbilityImage.color = new Color(255, 255, 255, transparencyOnCooldown);
                break;
        }

    }

    public void UseStack()
    {
        currentStacks--;
        if (currentStacks > 0)
        {
            powerUpStatus = Enum_PowerUpStatus.available;
        }
        else
        {
            powerUpStatus = Enum_PowerUpStatus.onCooldown;
        }
        ResetPowerUp();
        reloadStackCoroutine = StartCoroutine(ReloadStack());
        
    }

    public void ResetPowerUp()
    {
        statusEffect = Enum_SpecialStatus.normal;
    }

    public void GetRandomPowerUp()
    {
        statusEffect = (Enum_SpecialStatus)Random.Range(1, 4); 
    }

    private IEnumerator ReloadStack()
    {
        reloading = true;
        float elapsed = 0.0f;
        while(elapsed <= cooldown)
        {
            elapsed += Time.deltaTime;
            powerUpImageSlider.fillAmount = Mathf.Min(elapsed / cooldown, 1.0f);
            yield return null;
        }
        currentStacks = Mathf.Min(currentStacks + 1, maxStacks);
        powerUpStatus = Enum_PowerUpStatus.available;
        reloading = false;
    }
    public Enum_SpecialStatus StatusEffect
    {
        get
        {
            return statusEffect;
        }
    }

}
