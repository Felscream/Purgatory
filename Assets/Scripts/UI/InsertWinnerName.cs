using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsertWinnerName : MonoBehaviour {

    private InputField playerName;
    private ManagerInGame gameManager;

    private void Start()
    {
        gameManager = ManagerInGame.GetInstance();
        playerName = GetComponent<InputField>();
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
            gameManager.GetWinner().playerName = playerName.text.ToUpper();
        }
    }
}
