﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerInGame : MonoBehaviour {

    public GameObject Health;
    public GameObject Stamina;
    public GameObject Orb;
    private float Timer = 0.0f;
    private bool SpawnHealth;
    private bool SpawnStamina;
    private bool SpawnOrb;
    private int playerAlive = 0;
    public Component[] Players;
    private static ManagerInGame instance = null;
    protected Slider ClashSlider;
    protected GameObject background;
    protected GameObject canvas;
    public GameObject ClashHUD;
    protected GameObject cameraGo = null;
    private CameraControl cameraController;
    protected float aBaseRate;
    protected float dBaseRate;
    [SerializeField] protected int clashTime = 10;
    [SerializeField] protected float clashZoomDuration = 0.3f;
    [SerializeField] protected int defenderHealthGain = 30;
    [SerializeField] protected int attackerHealthLoss = 10;
    [SerializeField] protected float defenderImmunityTime = 1.5f;
    [SerializeField] protected GameObject attackerAura;
    [SerializeField] protected GameObject defenderAura;
    [SerializeField] protected GameObject backgroundEffect;

    //camera variables
    private float defaultOrthographicSize;
    private float defaultZoomOrthographicSize;
    public static ManagerInGame GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(ManagerInGame));
            return null;
        }
        return instance;
    }

    void Awake(){
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Players = GetComponentsInChildren<Champion>();
	}

    private void Start()
    {
        CheckPlayerAlive();
        ClashSlider = ClashHUD.GetComponentInChildren<Slider>();
        background = ClashHUD.GetComponentInChildren<SpriteRenderer>().gameObject;
        canvas = ClashHUD.GetComponentInChildren<Canvas>().gameObject;
        cameraGo = Camera.main.gameObject;
        /*GameObject[] gos = GameObject.FindGameObjectsWithTag("MainCamera");

        foreach( GameObject go in gos )
        {
            if (go.GetComponent<Camera>())
                cameraGo = go;
        }*/
        cameraController = cameraGo.GetComponent<CameraControl>();
        if(cameraController != null)
        {
            defaultOrthographicSize = cameraController.DefaultOrthographicSize;
            defaultZoomOrthographicSize = cameraController.ZoomOrthographicSize;
        }
    }
    void Update () {
        CheckPlayerAlive();
        Timer += Time.deltaTime;
		//Apparition des items sur la map à partir des prefabs
		if (Timer >= 5 && !SpawnHealth) {
			Instantiate (Health);
			SpawnHealth = true;
		}

		if (Timer >= 5 && !SpawnOrb) {
			Instantiate (Orb);
			//Instantiate (Plateform);
			SpawnOrb = true;
		}

		if (Timer >= 5 && !SpawnStamina) {
			Instantiate (Stamina);
			SpawnStamina = true;
		}

		
			
		/*if (playerAlive == 1) {   //A laisser en commentaire tant que la scène ne se lance pas depuis le menu de séléction de personnages
			SceneManager.LoadScene (1);
			//ici ajouter le changement de scène et toute les modifs à prendre en compte
		}*/
	}

    public IEnumerator ClashRoutine(Champion defender, Champion attacker)
    {
        Vector3 finalPos = new Vector3
        {
            x = (attacker.Position.x + defender.Position.x) / 2,
            y = (attacker.Position.y + defender.Position.y) / 2,
            z = 0
        };
        Vector3 startingPos = cameraController.MainAxis.position;
        Color color;
        float alpha = 0;
        float time = 0;
        int value = 50;
        float zd = cameraController.ZoomDuration;

        yield return new WaitForEndOfFrame();

        float orthographicSize = ComputeOrthographicSize(defender.Position, attacker.Position);
        //Start of clash
        Time.timeScale = 0.0001f;
        defender.ClashMode();
        attacker.ClashMode();

        //Animation at the start of a clash
        StartCoroutine(cameraController.ZoomIn(finalPos, clashTime, orthographicSize, clashZoomDuration));
        ClashHUD.SetActive(true);
        background.SetActive(true);
        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime*2;
            color = background.GetComponent<SpriteRenderer>().color;
            color.a = alpha;
            background.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }

        //Creates Aura and prep what's neeeded
        GameObject attackAura = Instantiate(attackerAura, attacker.transform);
        GameObject defendAura = Instantiate(defenderAura, defender.transform);
        GameObject bkg = Instantiate(backgroundEffect, cameraGo.transform);
        aBaseRate = attackAura.GetComponent<ParticleSystem>().emission.rateOverTime.constant;
        dBaseRate = defendAura.GetComponent<ParticleSystem>().emission.rateOverTime.constant;
        
        //Actual Clash
        canvas.gameObject.SetActive(true);
        while (time < clashTime && value < 100 && value > 0)
        {
            time += Time.unscaledDeltaTime;
            value = 50 + (attacker.clashClick * (10+attacker.determination) - defender.clashClick * (10+defender.determination))/10;
            ClashSlider.value = value;

            AuraManager(value, attackAura, defendAura);

            cameraController.Shake(Mathf.Abs(value-50) / 10f, 5, 1000);
            yield return null;
        }
        if (value >= 50)
        {
            defender.ReduceHealth(defender.Health);
        }
        else
        {
            defender.determination--;
            defender.Health += defenderHealthGain;
            attacker.ReduceHealth(attackerHealthLoss);
        }
        
        //Animation at the end of a clash
        Destroy(attackAura);
        Destroy(defendAura);
        Destroy(bkg);
        if (cameraController.isZooming)
        {
            StopCoroutine("cameraController.ZoomIn");
            StartCoroutine(cameraController.ZoomOut(startingPos));
        }
        cameraController.ZoomDuration = zd;
        alpha = 1;
        canvas.gameObject.SetActive(false);
        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime*2;
            color = background.GetComponent<SpriteRenderer>().color;
            color.a = alpha;
            background.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
        background.SetActive(false);
        ClashHUD.SetActive(false);

        Time.timeScale = 1f;
        defender.NormalMode();
        attacker.NormalMode();
        StartCoroutine(defender.ProcDivineShield(defenderImmunityTime));
    }

    protected void AuraManager(int value,GameObject attackAura, GameObject defendAura)
    {
        var attackerVel = attackAura.GetComponent<ParticleSystem>().limitVelocityOverLifetime;
        var defenderVel = defendAura.GetComponent<ParticleSystem>().limitVelocityOverLifetime;
        var attackerEmissionModule = attackAura.GetComponent<ParticleSystem>().emission;
        var defenderEmissionModule = defendAura.GetComponent<ParticleSystem>().emission;

        //Change Shape of auras depending of domination
        attackerVel.limitX = Mathf.Max((value / 10) - 5, 0);
        defenderVel.limitX = Mathf.Max(((100 - value) / 10) - 5, 0);
        attackerVel.dampen = 0.2f + ((0.2f * (float)(50 - value)) / 50);
        defenderVel.dampen = 0.2f + ((0.2f * (float)(value - 50)) / 50);
        attackerEmissionModule.rateOverTime = aBaseRate + (value - 50);
        defenderEmissionModule.rateOverTime = dBaseRate - (value - 50);
        
        //Dominating Aura is over the other
        if (value >= 50)
        {
            attackAura.GetComponent<Renderer>().sortingOrder = 9;
            defendAura.GetComponent<Renderer>().sortingOrder = 8;
        }
        else
        {
            attackAura.GetComponent<Renderer>().sortingOrder = 8;
            defendAura.GetComponent<Renderer>().sortingOrder = 9;
        }

    }

    public void CheckPlayerAlive()
    {
        int temp = 0;
        Players = GetComponentsInChildren<Champion>();
        foreach (Champion player in Players)
        {
            if (player != null && !player.Dead)
            {
                temp++;
            }
        }
        playerAlive = temp;
    }

    public int PlayerAlive
    {
        get
        {
            return playerAlive;
        }
    }

    public IEnumerator UltimateCameraEffect(Vector2 position, float waitTime)
    {
        Time.timeScale = 0.0001f;
        StartCoroutine(cameraController.ZoomIn(position, waitTime));
        yield return new WaitForSecondsRealtime(cameraController.ZoomDuration * 2 + waitTime);
        Time.timeScale = 1.0f;
    }

    public float ComputeOrthographicSize(Vector2 pOne, Vector2 pTwo)
    {
        float orthographicSize;
         
        Vector2 pTwoTemp = new Vector2(pTwo.x, pOne.y);
        float distance = Vector2.Distance(pOne, pTwoTemp) + 2; //le +2 donne juste de la marge pour faire apparaitre les persos à l'écran
        float horRes = (distance / 40) * 1920;  //utiliser Screen.Width si on veut changer la résolution
        float vertRes = horRes * (9.0f / 16.0f); //utiliser Screen.Width / Screen.Height si on veut changer la résolution
        orthographicSize = vertRes / (32 * 1.5f) * 0.5f; //Mettre 32 (Pixel Per Unit) et 1.5f (Pixel Scale) en constante quelque part
        orthographicSize = orthographicSize < defaultZoomOrthographicSize ? defaultZoomOrthographicSize : orthographicSize;
        orthographicSize = orthographicSize > defaultOrthographicSize ? defaultOrthographicSize : orthographicSize;
        
        return orthographicSize;
    }
}