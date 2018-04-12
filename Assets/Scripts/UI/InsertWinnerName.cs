using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        if(playerName.text != null && playerName.text != "")
        {
            playerName.text = playerName.text.ToUpper();
        }
        if (playerName.isFocused)
        {
            EndGameInput.GetInstance().activeMenu = false;
        }
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
        EndGameInput.GetInstance().activeMenu = true;
    }

    public void InputFieldSelected()
    {
        EndGameInput.GetInstance().activeMenu = false;
        EventSystem.current.SetSelectedGameObject(null);
        
    }
    
}
