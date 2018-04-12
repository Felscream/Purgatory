using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class VictoryMenu: MonoBehaviour {

    // ChampionsSelected instance
    ScoreManager scoreManager;
    ManagerInGame gameManager;
    private Enum_Champion playerSelection;

    // La tableau sur lequel on met le script
    public int tableIndex;
    public InputField winnerNameInput;
    // Les sprites
    public RectTransform playerTable;
    public RectTransform knightImage;
    public RectTransform archerImage;
    public RectTransform sorcererImage;
    // Les icônes
    public RectTransform knightIcon;
    public RectTransform archerIcon;
    public RectTransform sorcererIcon;
    // Les noms
    public RectTransform player_1;
    public RectTransform player_2;
    public RectTransform player_3;
    public RectTransform player_4;
    // Le score
    public GameObject ScoreText;

    // Calculate score
   // public Image ouroborosCooldown;

    // Use this for initialization
    void Start () {
        // Instance for prefab
        scoreManager = ScoreManager.GetInstance();
        gameManager = ManagerInGame.GetInstance();
    }

    public void CalculateScore()
    {

        Score winner = gameManager.GetWinner();
        List<Score> losers = gameManager.GetLosers();
        switch (tableIndex)
        {
            case 1: // Première place
                
                playerSelection = winner.champion;
                ScoreText.GetComponent<Text>().text = winner.TotalScore.ToString() ;
                Affichage(playerSelection, winner.playerID);
                if ( CanEnterLeaderboard() && winnerNameInput != null)
                {
                    winnerNameInput.gameObject.SetActive(true);
                }
                break;
            case 2: // Deuxième place
                if(losers.Count >= 1)
                {
                    playerSelection = losers[0].champion;
                    ScoreText.GetComponent<Text>().text = losers[0].TotalScore.ToString();
                    Affichage(playerSelection, losers[0].playerID);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case 3: // Troisième place
                if (losers.Count >= 2)
                {
                    playerSelection = losers[1].champion;
                    ScoreText.GetComponent<Text>().text = losers[1].TotalScore.ToString();
                    Affichage(playerSelection, losers[1].playerID);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case 4: // Quatrième place
                if (losers.Count >= 3)
                {
                    playerSelection = losers[2].champion;
                    ScoreText.GetComponent<Text>().text = losers[2].TotalScore.ToString();
                    Affichage(playerSelection, losers[2].playerID);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            default: // if 0, nothing selected
                break;
        }
    }

    private void Affichage(Enum_Champion playerSelection, int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                player_1.gameObject.SetActive(true);
                player_2.gameObject.SetActive(false);
                player_3.gameObject.SetActive(false);
                player_4.gameObject.SetActive(false);
                break;
            case 1:
                player_1.gameObject.SetActive(false);
                player_2.gameObject.SetActive(true);
                player_3.gameObject.SetActive(false);
                player_4.gameObject.SetActive(false);
                break;
            case 2:
                player_1.gameObject.SetActive(false);
                player_2.gameObject.SetActive(false);
                player_3.gameObject.SetActive(true);
                player_4.gameObject.SetActive(false);
                break;
            case 3:
                player_1.gameObject.SetActive(false);
                player_2.gameObject.SetActive(false);
                player_3.gameObject.SetActive(false);
                player_4.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }

        switch (playerSelection)
        {
            case Enum_Champion.Knight:
                // Changer sprite
                knightImage.gameObject.SetActive(true);
                sorcererImage.gameObject.SetActive(false);
                archerImage.gameObject.SetActive(false);
               /* knightIcon.gameObject.SetActive(true);
                sorcererIcon.gameObject.SetActive(false);
                archerIcon.gameObject.SetActive(false);*/
                break;
            case Enum_Champion.Sorcerer:
                // Changer sprite
                knightImage.gameObject.SetActive(false);
                sorcererImage.gameObject.SetActive(true);
                archerImage.gameObject.SetActive(false);
                /*knightIcon.gameObject.SetActive(false);
                sorcererIcon.gameObject.SetActive(true);
                archerIcon.gameObject.SetActive(false);*/
                break;
            case Enum_Champion.Archer:
                // Changer sprite
                knightImage.gameObject.SetActive(false);
                sorcererImage.gameObject.SetActive(false);
                archerImage.gameObject.SetActive(true);
                /*knightIcon.gameObject.SetActive(false);
                sorcererIcon.gameObject.SetActive(false);
                archerIcon.gameObject.SetActive(true);*/
                break;
            default:
                playerTable.gameObject.SetActive(false);
                break;
                /*default:
                    print("Incorrect intelligence level.");
                    break;*/
        }
    }

    // Update is called once per frame
    void Update () {
        /*
        while (ouroborosCooldown.fillAmount > 0)
        {
            //Reduce fill amount over 30 seconds
            ouroborosCooldown.fillAmount -= 0.5f * Time.deltaTime;
        }
        CalculateScoreImageRefill();*/
    }

    public void CalculateScoreImageRefill()
    {
        //ouroborosCooldown.fillAmount = 1.0f;
    }

    private bool CanEnterLeaderboard()
    {
        return gameManager.GetWinner().TotalScore > scoreManager.GetLastOnLeaderboard() || scoreManager.leaderboard.Count < scoreManager.leaderboardSize;
    }
}
