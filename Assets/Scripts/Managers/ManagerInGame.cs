using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerInGame : MonoBehaviour {

    public GameObject Health;
    public GameObject Stamina;
    public GameObject Orb;
    public float Timer = 0.0f;
    private bool SpawnHealth;
    private bool SpawnStamina;
    private bool SpawnOrb;
	private bool SpawnItem;
    private int playerAlive = 0;
    public Champion[] Players;
    private static ManagerInGame instance = null;
    protected Slider ClashSlider;
    protected GameObject background;
    protected GameObject canvas;
    public GameObject ClashHUD;
    protected GameObject cameraGo = null;
    private CameraControl cameraController;
    private AudioVolumeManager audioManager;
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
    [SerializeField] private float ouroborosFirstShakeTimer;
    [SerializeField] private int firstShakes;
    [SerializeField] private float ouroborosSecondShakeTimer;
    [SerializeField] private int secondShakes;
    [SerializeField] private float startCountDown = 3.0f;
    [SerializeField] private Text countDownUI;

    private List<AudioSource> agentsAudioSources = new List<AudioSource>();

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
        audioManager = AudioVolumeManager.GetInstance();
        
    }
    void Update () {
        CheckPlayerAlive();
        Timer += Time.deltaTime;
		//Apparition des items sur la map à partir des prefabs
		if (Timer >= 5 && !SpawnItem) {
			InvokeRepeating ("SpawningItems",1,60);
			SpawnItem = true;
		}

		if (Timer >= 5 && !SpawnOrb) {
			Instantiate (Orb);
			//Instantiate (Plateform);
			SpawnOrb = true;
		}
        /*if (playerAlive == 1) {   //A laisser en commentaire tant que la scène ne se lance pas depuis le menu de séléction de personnages
			SceneManager.LoadScene (1);
            Narrator.Instance.End();
			//ici ajouter le changement de scène et toute les modifs à prendre en compte
		}*/
    }

    void SpawningItems(){
	
		Instantiate (Health, getValidSpawnPosition (), Quaternion.identity);
		Instantiate (Stamina, getValidSpawnPosition (), Quaternion.identity);
	}

	Vector2 getValidSpawnPosition(){
		Vector2 newPosition = Vector2.zero;
		switch (Random.Range(1,5)) {
				case 1:
					newPosition = new Vector2(Random.Range(-19f, 19f), Random.Range(5.5f, 10f));
					break;

				case 2:
					newPosition = new Vector2(Random.Range(-19f, 19f), Random.Range(1.4f, 3f));
					break;

				case 3:
					newPosition = new Vector2(Random.Range(-19f, 19f), Random.Range(-2.9f, -1.5f));
					break;

				case 4:
					newPosition = new Vector2(Random.Range(-19f, 19f), Random.Range(-7.3f, -5.2f));
					break;
		}
        // return the valid position
        return newPosition;
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
        float value = 50;
        float zd = cameraController.ZoomDuration;

        yield return new WaitForEndOfFrame();

        float orthographicSize = ComputeOrthographicSize(defender.Position, attacker.Position);
        //Start of clash
        PauseAgentsAudio();
        Time.timeScale = 0.0001f;
        audioManager.PlaySoundEffect("ExecutionStart");
        defender.ClashMode();
        attacker.ClashMode();

        //Animation at the start of a clash
        StartCoroutine(cameraController.ZoomIn(finalPos, clashTime, orthographicSize, clashZoomDuration));
        GetComponentInChildren<AddChampion>().HUDPlayer1.gameObject.SetActive(false);
        GetComponentInChildren<AddChampion>().HUDPlayer2.gameObject.SetActive(false);
        GetComponentInChildren<AddChampion>().HUDPlayer3.gameObject.SetActive(false);
        GetComponentInChildren<AddChampion>().HUDPlayer4.gameObject.SetActive(false);
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
            value = 50 + ((attacker.clashClick * 10 - defender.clashClick * (7+defender.determination)) * (time/2+1) / 10);
            ClashSlider.value = value;

            AuraManager(value, attackAura, defendAura);
            
            cameraController.Shake(/*Mathf.Abs(value-50) / 2f + */ time*4, 5, 1000);
            
            yield return null;
        }
        if (value >= 50)
        {
            defender.ReduceHealth(defender.Health);
            audioManager.PlaySoundEffect("DefenseLoss");
            defender.Controller.AddRumble(0.2f, new Vector2(.9f,.9f), 0.2f);
        }
        else
        {
            defender.determination--;
            defender.Health += defenderHealthGain;
            attacker.ReduceHealth(attackerHealthLoss);
            attacker.Controller.AddRumble(0.2f, new Vector2(.9f, .9f), 0.2f);
            audioManager.PlaySoundEffect("DefenseWin");
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
        GetComponentInChildren<AddChampion>().HUDPlayer1.gameObject.SetActive(true);
        GetComponentInChildren<AddChampion>().HUDPlayer2.gameObject.SetActive(true);
        GetComponentInChildren<AddChampion>().HUDPlayer3.gameObject.SetActive(true);
        GetComponentInChildren<AddChampion>().HUDPlayer4.gameObject.SetActive(true);
        background.SetActive(false);
        ClashHUD.SetActive(false);

        Time.timeScale = 1f;
        UnpauseAgentsAudio();
        defender.NormalMode();
        attacker.NormalMode();
        StartCoroutine(defender.ProcDivineShield(defenderImmunityTime));
    }

    protected void AuraManager(float value,GameObject attackAura, GameObject defendAura)
    {
        var attackerVel = attackAura.GetComponent<ParticleSystem>().limitVelocityOverLifetime;
        var defenderVel = defendAura.GetComponent<ParticleSystem>().limitVelocityOverLifetime;
        var attackerEmissionModule = attackAura.GetComponent<ParticleSystem>().emission;
        var defenderEmissionModule = defendAura.GetComponent<ParticleSystem>().emission;

        //Change Shape of auras depending of domination
        attackerVel.limitX = Mathf.Max((value / 10) - 5, 0);
        defenderVel.limitX = Mathf.Max(((100 - value) / 10) - 5, 0);
        attackerVel.dampen = 0.2f + ((0.3f * (50 - value)) / 50);
        defenderVel.dampen = 0.2f + ((0.3f * (value - 50)) / 50);
        attackerEmissionModule.rateOverTime = aBaseRate + (value - 50)*1.5f;
        defenderEmissionModule.rateOverTime = dBaseRate - (value - 50)*1.5f;
        
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

    public void AddAudioSource(AudioSource a)
    {
        agentsAudioSources.Add(a);
    }

    public void RemoveAudioSource(AudioSource a)
    {
        agentsAudioSources.Remove(a);
    }

    public void PauseAgentsAudio()
    {
        foreach(AudioSource a in agentsAudioSources)
        {
            a.Pause();
        }
    }

    public void UnpauseAgentsAudio()
    {
        foreach(AudioSource a in agentsAudioSources)
        {
            a.UnPause();
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

    public IEnumerator UltimateCameraEffect(Vector2 position, float waitTime, Champion champion)
    {
        PauseAgentsAudio();
        Time.timeScale = 0.0001f;
        champion.animator.speed = 1 / Time.timeScale;
        StartCoroutine(cameraController.ZoomIn(position, waitTime));
        yield return new WaitForSecondsRealtime(cameraController.ZoomDuration * 2 + waitTime);
        Time.timeScale = 1.0f;
        champion.EndUltLoop();
        champion.animator.speed = Time.timeScale;
        UnpauseAgentsAudio();
    }

    public float ComputeOrthographicSize(Vector2 pOne, Vector2 pTwo)
    {
        float orthographicSize = defaultOrthographicSize;
        Vector2 pTwoTemp;
        if (Mathf.Abs(pOne.x - pTwo.x) > Mathf.Abs(pOne.y - pTwo.y))
        {
            pTwoTemp = new Vector2(pTwo.x, pOne.y);
            float distance = Vector2.Distance(pOne, pTwoTemp) + 2; //le +2 donne juste de la marge pour faire apparaitre les persos à l'écran
            float horRes = (distance / 40) * 1920;  //utiliser Screen.Width si on veut changer la résolution
            float vertRes = horRes * (9.0f / 16.0f); //utiliser Screen.Width / Screen.Height si on veut changer la résolution
            orthographicSize = vertRes / (32 * 1.5f) * 0.5f; //Mettre 32 (Pixel Per Unit) et 1.5f (Pixel Scale) en constante quelque part
            
        }
        else
        {
            pTwoTemp = new Vector2(pOne.x, pTwo.y);
            float distance = Vector2.Distance(pOne, pTwoTemp) + 8.0f;
            float vertRes = (distance / 25) * 1080;
            orthographicSize = vertRes / (32 * 1.5f) * 0.5f;
        }
        orthographicSize = orthographicSize < defaultZoomOrthographicSize ? defaultZoomOrthographicSize : orthographicSize;
        orthographicSize = orthographicSize > defaultOrthographicSize ? defaultOrthographicSize : orthographicSize;
        return orthographicSize;
    }

    public IEnumerator Ouroboros()
    {
        AddAudioSource(audioManager.ouroborosAudioSource);
        float timer = 0.0f;
        audioManager.PlaySoundEffect("Ouroboros");
        while (timer < ouroborosFirstShakeTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        cameraController.Shake(10, firstShakes, 100, true);
        foreach(Champion c in Players)
        {
            c.Controller.AddRumble(2.0f, new Vector2(0.6f, 0.6f), 2.0f);
        }
        while (timer < ouroborosSecondShakeTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        cameraController.Shake(10, secondShakes, 100, true);
        foreach (Champion c in Players)
        {
            c.Controller.AddRumble(1.5f, new Vector2(0.6f, 0.6f), 1.5f);
        }
    }

    public IEnumerator StartGame()
    {
        float timer = startCountDown;
        Animator anim = countDownUI.GetComponent<Animator>();
        while(timer > 0)
        {
            countDownUI.text = timer.ToString();
            countDownUI.gameObject.SetActive(true);
            anim.Play("countdown", -1, 0.0f);
            timer -= 1;
            yield return new WaitForSeconds(1);
            
            yield return null;
        }

        foreach(Champion champ in Players)
        {
            champ.hardBlock = false;
        }
        Narrator.Instance.StartOfTheGame();
    }
}
