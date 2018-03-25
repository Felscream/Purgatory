using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionsSelected : MonoBehaviour {

    private static ChampionsSelected instance;
    private int player1_indexSelection;
    private int player2_indexSelection;
    private int player3_indexSelection;
    private int player4_indexSelection;
    private int playerNumber;

    void Awake()
    {
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

    public static ChampionsSelected GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(ChampionsSelected));
            return null;
        }
        return instance;
    }

    public int Player1_indexSelection
    {
        get
        {
            return player1_indexSelection;
        }

        set
        {
            player1_indexSelection = value;
        }
    }

    public int Player2_indexSelection
    {
        get
        {
            return player2_indexSelection;
        }

        set
        {
            player2_indexSelection = value;
        }
    }

    public int Player3_indexSelection
    {
        get
        {
            return player3_indexSelection;
        }

        set
        {
            player3_indexSelection = value;
        }
    }

    public int Player4_indexSelection
    {
        get
        {
            return player4_indexSelection;
        }

        set
        {
            player4_indexSelection = value;
        }
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
