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
    private int PlayerAlive = 0;
    public Component[] Players;
    private static ManagerInGame instance = null;
    public Slider ClashSlider;
    public Canvas ClashCanvas;
    public int attackerClick, defenderClick;

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

	void Update () {
		PlayerAlive = 0;
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

		foreach (Champion player in Players) {
			if (player != null) {
				if (player.Health > 0) {
					PlayerAlive += 1;
				}
			}
		}
			
		if (PlayerAlive == 1) {
			SceneManager.LoadScene (1);
			//ici ajouter le changement de scène et toute les modifs à prendre en compte
		}
	}

    public void Clash(Champion defender, Champion attacker)
    {
        ClashCanvas.gameObject.SetActive(true);
        ClashSlider.gameObject.SetActive(true);

        Time.timeScale = 0.0001f;

        defender.ClashMode();
        attacker.ClashMode();
        float time = 0;
        int value = 0;
        while (time < 10 && value<100 && value>0)
        {
            time += Time.unscaledDeltaTime;
            value = 50 + (attacker.clashClick * attacker.determination - defender.clashClick * defender.determination);
            ClashSlider.value = value;
        }
        if(value >= 50)
        {
            defender.ReduceHealth(1);
        }
        else
        {
            defender.determination--;
            defender.Health += 30;
            attacker.ReduceHealth(15);
        }

        ClashCanvas.gameObject.SetActive(false);
        ClashSlider.gameObject.SetActive(false);

        Time.timeScale = 1f;

        defender.NormalMode();
        attacker.NormalMode();
    }
    
}
