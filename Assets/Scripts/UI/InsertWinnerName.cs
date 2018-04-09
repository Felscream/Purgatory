using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsertWinnerName : MonoBehaviour {

    private InputField playerName;
    private ManagerInGame gameManager;
    private ScoreManager scoreManager;
    private ScoreData winnerData;
    private void Start()
    {
        gameManager = ManagerInGame.GetInstance();
        scoreManager = ScoreManager.GetInstance();
        
        playerName = GetComponent<InputField>();
        winnerData = scoreManager.AddChallengerToLeaderboard(gameManager.GetWinner());
    }

    private void Update()
    {
        playerName.text = playerName.text.ToUpper();
    }
    // Use this for initialization
    public void GetWinnerName()
    {
        playerName.text = gameManager.GetWinner().playerName;
    }

    public void SetWinnerName()
    {
        if(gameManager.GetWinner() != null)
        {
            winnerData.playerName = playerName.text.ToUpper();
        }
    }
}
