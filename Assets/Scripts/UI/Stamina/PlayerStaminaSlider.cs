using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaSlider : MonoBehaviour {

    [SerializeField]
    private Vector3 offset = new Vector3(-2, 0.4f, 0);
    [SerializeField]
    private float timeToFade = 2.0f, fadingDuration = 0.5f;
    [SerializeField]
    private Sprite[] sliders = new Sprite[2], fillers = new Sprite[2];
    [SerializeField]
    private Transform player;
    private Champion champion;
    private Slider staminaSlider;
    private Image background;
    private Image filler;
    private float fadeTimer;
    // Use this for initialization
    void Start () {
        background = transform.Find("Background").GetComponentInChildren<Image>();
        filler = transform.Find("Fill Area").GetComponentInChildren<Image>();
        champion = FindComponentInChildWithTag<Champion>(player, "Champion");
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

    public static T FindComponentInChildWithTag<T>(Transform parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
}
