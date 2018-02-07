using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSlider : MonoBehaviour {

    [SerializeField]
    private Vector3 offset = new Vector3(-2, 0.4f, 0);
    [SerializeField] private Transform player;
    private Champion champion;
    private Slider healthSlider;
    // Use this for initialization
    void Start()
    {
        champion = FindComponentInChildWithTag<Champion>(player, "Champion");
        healthSlider = GetComponent<Slider>();
        healthSlider.maxValue = champion.BaseHealth;
        healthSlider.minValue = 0.0f;
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
            if (facing > 0)
            {
                offset.x = -Mathf.Abs(offset.x);
            }
        }
        transform.position = Camera.main.WorldToScreenPoint(champion.transform.position + (Vector3)offset);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        healthSlider.value = champion.Health;
        if(healthSlider.value == 0)
        {
            healthSlider.transform.Find("Background").GetComponent<Image>().color = Color.black;
        }
    }

    // Trouve l'enfant avec le tag 'tag'
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
