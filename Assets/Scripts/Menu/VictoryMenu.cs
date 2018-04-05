using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class VictoryMenu: MonoBehaviour {

    // ChampionsSelected instance
    ChampionManager championsSelected_;
    private int playerSelection;
    // Tous les scores
    private int[] playerScore;
    private int[] playerScoreOrder;

    // La tableau sur lequel on met le script
    public int tableIndex;
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
        championsSelected_ = ChampionManager.GetInstance();

        for (int i=0; i < championsSelected_.playerScore.Length; i++)
        {
            playerScore[i] = championsSelected_.playerSelection[i];
        }
        CalculateScore();
    }

    private void CalculateScore()
    {
        playerScoreOrder = playerScore;
        Array.Sort(playerScoreOrder);
        
        switch (tableIndex)
        {
            case 1: // Première place
                for (int i = 0; i < playerScore.Length; i++)
                {
                    if (playerScore[i] == playerScoreOrder[0])
                    {
                        playerSelection = championsSelected_.playerSelection[i];
                        ScoreText.GetComponent<Text>().text = "Score : " + playerScoreOrder[0];
                        Affichage(playerSelection, i);
                    }
                }
                break;
            case 2: // Deuxième place
                for (int i = 0; i < playerScore.Length; i++)
                {
                    if (playerScore[i] == playerScoreOrder[1])
                    {
                        playerSelection = championsSelected_.playerSelection[i];
                        ScoreText.GetComponent<Text>().text = "Score : " + playerScoreOrder[1];
                        Affichage(playerSelection, i);
                    }
                }
                break;
            case 3: // Troisième place
                for (int i = 0; i < playerScore.Length; i++)
                {
                    if (playerScore[i] == playerScoreOrder[2])
                    {
                        playerSelection = championsSelected_.playerSelection[i];
                        ScoreText.GetComponent<Text>().text = "Score : " + playerScoreOrder[2];
                        Affichage(playerSelection, i);
                    }
                }
                break;
            case 4: // Quatrième place
                for (int i = 0; i < playerScore.Length; i++)
                {
                    if (playerScore[i] == playerScoreOrder[3])
                    {
                        playerSelection = championsSelected_.playerSelection[i];
                        ScoreText.GetComponent<Text>().text = "Score : " + playerScoreOrder[3];
                        Affichage(playerSelection, i);
                    }
                }
                break;
            default: // if 0, nothing selected
                break;
        }
    }

    private void Affichage(int playerSelection, int playerIndex)
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
            case 0:
                playerTable.gameObject.SetActive(false);
                break;
            case 1:
                // Changer sprite
                knightImage.gameObject.SetActive(true);
                sorcererImage.gameObject.SetActive(false);
                archerImage.gameObject.SetActive(false);
                knightIcon.gameObject.SetActive(true);
                sorcererIcon.gameObject.SetActive(false);
                archerIcon.gameObject.SetActive(false);
                break;
            case 2:
                // Changer sprite
                knightImage.gameObject.SetActive(false);
                sorcererImage.gameObject.SetActive(true);
                archerImage.gameObject.SetActive(false);
                knightIcon.gameObject.SetActive(false);
                sorcererIcon.gameObject.SetActive(true);
                archerIcon.gameObject.SetActive(false);
                break;
            case 3:
                // Changer sprite
                knightImage.gameObject.SetActive(false);
                sorcererImage.gameObject.SetActive(false);
                archerImage.gameObject.SetActive(true);
                knightIcon.gameObject.SetActive(false);
                sorcererIcon.gameObject.SetActive(false);
                archerIcon.gameObject.SetActive(true);
                break;
            default:
                print("Incorrect intelligence level.");
                break;
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
}
