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
    public Canvas ClashCanvas;
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
        ClashSlider = ClashCanvas.GetComponentInChildren<Slider>();
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
        ClashCanvas.gameObject.SetActive(true);
        ClashSlider.gameObject.SetActive(true);
        
        Time.timeScale = 0.0001f;

        defender.ClashMode();
        attacker.ClashMode();
        float time = 0;
        int value = 50;

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

        ClashCanvas.gameObject.SetActive(false);
        ClashSlider.gameObject.SetActive(false);

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
}
