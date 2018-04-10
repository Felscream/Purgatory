using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChampionManager: MonoBehaviour {

    private static ChampionManager instance;
    //private int player1_indexSelection = 0, player2_indexSelection = 0, player3_indexSelection = 0, player4_indexSelection = 0;
    private int playerNumber;

    public string Start_P1;
    private bool lobbySelected = false;
    private AudioVolumeManager audioVolumeManager;
    public int[] playerSelection;

    void Awake()
    {
        playerSelection = new int[4];
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // we want to keep it the whole time the game is running
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Player 1 index = 0;
        playerSelection[0] = 0;
        // Player 2 index = 0;
        playerSelection[1] = 0;
        // Player 3 index = 0;
        playerSelection[2] = 0;
        // Player 4 index = 0;
        playerSelection[3] = 0;


        audioVolumeManager = AudioVolumeManager.GetInstance();
        audioVolumeManager.PlayTheme("LobbyTheme");
    }

    public static ChampionManager GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(ChampionManager));
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
