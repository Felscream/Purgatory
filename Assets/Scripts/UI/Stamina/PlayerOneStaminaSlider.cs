using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOneStaminaSlider : MonoBehaviour {

    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float size, lockSpeed, timeToFade = 2.0f, fadingDuration = 0.5f;
    [SerializeField]
    private Sprite[] sliders, fillers;
    [SerializeField]
    private Transform target;
    private Champion champion;
    private Slider staminaSlider;
    private Image background;
    private Image filler;
    private Vector3 velocity = Vector3.zero;
    private float fadeTimer;
    // Use this for initialization
    void Start () {
        background = transform.Find("Background").GetComponentInChildren<Image>();
        filler = transform.Find("Fill Area").GetComponentInChildren<Image>();
        champion = target.GetComponentInChildren<Champion>();
        staminaSlider = GetComponent<Slider>();
        staminaSlider.maxValue = champion.BaseStamina;
        staminaSlider.minValue = 0.0f;
        fadeTimer = timeToFade;
    }

    private void FixedUpdate()
    {
        float facing = champion.Facing;
        if (facing < 0)
        {
            offset.x = Mathf.Abs(offset.x);
        }
        else
        {
            if(facing > 0)
            {
                offset.x = -Mathf.Abs(offset.x);
            }
        }
        transform.position = Camera.main.WorldToScreenPoint(champion.transform.position + (Vector3)offset);
    }
    // Update is called once per frame
    void LateUpdate () {
        staminaSlider.value = champion.Stamina;
        if(staminaSlider.value == champion.BaseStamina)
        {
            fadeTimer += Time.deltaTime;
            if(fadeTimer >= timeToFade)
            {
                background.CrossFadeAlpha(0, fadingDuration, true);
                filler.CrossFadeAlpha(0, fadingDuration, true);
            }
        }
        else
        {
            background.CrossFadeAlpha(1, 0.1f, true);
            filler.CrossFadeAlpha(1, 0.1f, true);
            fadeTimer = 0.0f;
        }
        if (champion.Fatigue)
        {
            background.sprite = sliders[1];
            filler.sprite = fillers[1];
        }
        else
        {
            background.sprite = sliders[0];
            filler.sprite = fillers[0];
        }
	}
}
