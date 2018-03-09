using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakableLife : MonoBehaviour {

    public int maxLife = 4;
    private int life = 4;
    public float colorChangeTime = .2f;
    public Transform ascendant;
    public Transform descendant;
    public Transform falling;
    private bool    hasFallen = false,
                    isLowest = false,
                    beginLowering = false;

    private Color initialColor;
    private SpriteRenderer leverColor;

    // Timer to change the color of the breakable when hit
    private float timerColor;

    // Timer to low the plateform
    private float timer;


	// Use this for initialization
	void Start () {
        initialColor = new Color(0, 255, 255);
        leverColor = this.gameObject.GetComponent<SpriteRenderer>();
        leverColor.color = initialColor;
        ascendant.gameObject.SetActive(true);
        descendant.gameObject.SetActive(true);
        falling.gameObject.SetActive(false);
        timer = 0.0f;
        timerColor = colorChangeTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!beginLowering)
        {
            timer += Time.deltaTime;
            if (timer >= 8) // Ammorce la descente après 5 secondes
            {
                beginLowering = true;
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
                timer = 0.0f;
            }
        }
        else
        {
            if (!hasFallen && !isLowest)
            {
                timer += Time.deltaTime;
                ascendant.localPosition = new Vector3(1, Random.value * 0.1f, 0);
                descendant.localPosition = new Vector3(0, 10 - timer/2, 0);            // Abaisse la plateforme jusqu'à un offset vertical de 0
            }
            if (descendant.position.y <= 0)
            {
                isLowest = true;
            }
        }
        if (timerColor < colorChangeTime)
        {
            leverColor.color = new Color(255, 0, 0);
            timerColor += Time.deltaTime;
        } else
        {
            leverColor.color = initialColor;
        }
	}

    public void TakeDamage(int dmg)
    {
        if (beginLowering)
        { 
            Debug.Log("Take damage");
            life -= dmg;
            if (life <= 0)
            {
                Fall();
                hasFallen = true;
            }
            timerColor = 0f;
        }
    }

    void Fall()
    {
        falling.position = descendant.position;
        falling.gameObject.SetActive(true);
        ascendant.gameObject.SetActive(false);
        descendant.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        Destroy(falling.gameObject, 5f);
        
    }
}
