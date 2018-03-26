using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChampionsSelected : MonoBehaviour {

    private static ChampionsSelected instance;
    //private int player1_indexSelection = 0, player2_indexSelection = 0, player3_indexSelection = 0, player4_indexSelection = 0;
    private int playerNumber;

    public string Start_P1;
    private bool lobbySelected = false;

    public List<int> playerSelection = new List<int>();

    void Awake()
    {
        // Player 1 index = 0;
        playerSelection.Add(0);
        // Player 2 index = 0;
        playerSelection.Add(0);
        // Player 3 index = 0;
        playerSelection.Add(0);
        // Player 4 index = 0;
        playerSelection.Add(0);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //we want to keep it the whole time the game is running
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel()
    {
        if (Input.GetButtonDown(Start_P1) && !lobbySelected)
        {
            Debug.Log("Je passe ici");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            lobbySelected = true;
        }
    }

    public static ChampionsSelected GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(ChampionsSelected));
            return null;
        }
        return instance;
    }

    public int PlayerNumber
    {
        get
        {
            return playerNumber;
        }

        set
        {
            playerNumber = value;
        }
    }
}
