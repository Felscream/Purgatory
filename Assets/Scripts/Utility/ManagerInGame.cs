using System.Collections;
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
    [SerializeField] protected int clashTime = 10;
    [SerializeField] protected int defenderHealthGain = 30;
    [SerializeField] protected int attackerHealthLoss = 10;
    [SerializeField] protected float defenderImmunityTime = 1.5f;

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
        yield return new WaitForEndOfFrame();
        Time.timeScale = 0.0001f;
        defender.ClashMode();
        attacker.ClashMode();

        StartCoroutine(cameraController.ZoomIn(finalPos, clashTime));

        ClashHUD.SetActive(true);
        background.SetActive(true);
        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime*2;
            color = background.GetComponent<SpriteRenderer>().color;
            color.a = alpha;
            background.GetComponent<SpriteRenderer>().color = color;
            /*cameraGo.GetComponent<CameraControl>().Move(finalPos.x, finalPos.y, 11);
            /*
            temp = cameraGo.transform.position;
            temp = Vector3.MoveTowards(temp, finalPos, Vector3.Distance(temp, finalPos) * Time.unscaledDeltaTime * 2);
            temp.z = 0;
            cameraGo.transform.position = temp;
            Debug.Log(temp + " et " + cameraGo.transform.position);
            */
            yield return null;
        }
        canvas.gameObject.SetActive(true);
        
        while (time < clashTime && value < 100 && value > 0)
        {
            time += Time.unscaledDeltaTime;
            value = 50 + (attacker.clashClick * (10+attacker.determination) - defender.clashClick * (10+defender.determination))/10;
            ClashSlider.value = value;
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
        
        if(cameraController.isZooming)
        {
            StopCoroutine("cameraController.ZoomIn");
            StartCoroutine(cameraController.ZoomOut(startingPos));
        }

        alpha = 1;
        canvas.gameObject.SetActive(false);
        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime*2;
            color = background.GetComponent<SpriteRenderer>().color;
            color.a = alpha;
            background.GetComponent<SpriteRenderer>().color = color;
            /*cameraGo.GetComponent<CameraControl>().Move(startingPos.x, startingPos.y, 11);
            /*
            temp = cameraGo.transform.position;
            temp = Vector3.MoveTowards(temp, startingPos, Vector3.Distance(temp, startingPos) * Time.unscaledDeltaTime * 2);
            temp.z = 0;
            cameraGo.transform.position = temp;
            Debug.Log(temp + " et " + cameraGo.transform.position);
            */
            yield return null;
        }
        background.SetActive(false);
        ClashHUD.SetActive(false);


        Time.timeScale = 1f;

        defender.NormalMode();
        attacker.NormalMode();
        StartCoroutine(defender.ProcDivineShield(defenderImmunityTime));
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
}
