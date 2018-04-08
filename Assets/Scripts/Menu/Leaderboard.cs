using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour {

    private GameObject[] children;
    private ScoreManager scoreManager;
    private int plateSpacing = 30;
    [SerializeField] GameObject[] playerPlates = new GameObject[4];
    [SerializeField] GameObject[] characters = new GameObject[3];
    [SerializeField] GameObject noGameData;
    // Use this for initialization
    void Start () {
        scoreManager = ScoreManager.GetInstance();
        noGameData.SetActive(false);
        if (noGameData && scoreManager.leaderboard.Count == 0)
        {
            noGameData.SetActive(true);
            return;
        }
		for(int i = 0; i < scoreManager.leaderboard.Count; ++i)
        {
            switch (i)
            {
                case 0:
                    GameObject first = Instantiate(playerPlates[0], transform);
                    Instantiate(characters[(int)scoreManager.leaderboard[i].champion], first.transform);
                    ConfigurePlayerPlate(first, i);
                    break;
                case 1:
                    GameObject second = Instantiate(playerPlates[1], transform);
                    Instantiate(characters[(int)scoreManager.leaderboard[i].champion], second.transform);
                    ConfigurePlayerPlate(second, i);
                    break;
                case 2:
                    GameObject third = Instantiate(playerPlates[2], transform);
                    Instantiate(characters[(int)scoreManager.leaderboard[i].champion], third.transform);
                    ConfigurePlayerPlate(third, i);
                    break;
                default:
                    GameObject go = Instantiate(playerPlates[3], transform);
                    Vector3 pos = go.GetComponent<RectTransform>().localPosition;
                    go.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y + plateSpacing * (3 - i), pos.z);
                    go.transform.Find("Place").GetComponent<Text>().text = (i + 1).ToString();
                    ConfigurePlayerPlate(go, i);
                    break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ConfigurePlayerPlate(GameObject plate, int index)
    {
        plate.transform.Find("PlayerName").GetComponent<Text>().text = scoreManager.leaderboard[index].playerName;
        plate.transform.Find("Score").GetComponent<Text>().text = (scoreManager.leaderboard[index].totalScore).ToString();
    }
}
