using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;



public class ScoreManager : MonoBehaviour {
    private static ScoreManager instance;
    [NonSerialized] public List<ScoreData> leaderboard = new List<ScoreData>();
    [NonSerialized] public List<Score> challengers = new List<Score>();
    [NonSerialized] public bool gameStart = false;
    public int leaderboardSize = 10;
    public int executionPoints = 500;
    public int heartPoints = 150;
    public int executionResistancePoints = 375;
    public int pickupPoints = 50;
    public int parryPoints = 250;
    public int victoryPoints = 1000;
    public float timerBetweenIncrease = 20.0f;
    private string fileName = "scoreData.dat";
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static ScoreManager GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(ScoreManager));
            return null;
        }
        return instance;
    }
    private void OnEnable()
    {
        Load();
    }

    private void OnDisable()
    {
        Save();
    }

    private void LateUpdate()
    {
        if (gameStart)
        {
            foreach(Score s in challengers)
            {
                s.timer += Time.deltaTime;
                if(s.timer >= timerBetweenIncrease)
                {
                    s.IncreaseMultiplier();
                }
            }
        }        
    }

    public void AddChallengersToLeaderboard()
    {
        DateTime date = DateTime.Now;
        foreach(Score s in challengers)
        {
            ScoreData data = new ScoreData(s.totalScore, s.champion);
            leaderboard.Add(data);
        }
        leaderboard.Sort();
        if (leaderboard.Count > leaderboardSize)
        {
            int toRemove = leaderboardSize - leaderboard.Count;
            for (int i = toRemove; i == 1; --i)
            {
                leaderboard.RemoveAt(leaderboard.Count - 1);
            }
        }
    }
    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
        leaderboard.Sort();
        LeaderboardData data = new LeaderboardData(leaderboard);
        bf.Serialize(file, data);
        file.Close();
    }

    private void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            LeaderboardData data = (LeaderboardData)bf.Deserialize(file);
            file.Close();
            data.leaderboard.Sort();
            leaderboard = data.leaderboard;
        }
    }
}

public class Score : IEquatable<Score>, IComparable<Score>
{
    public int playerID;
    public int totalScore;
    public int multiplier;
    public Enum_Champion champion;
    public float timer;

    public Score(int id, Enum_Champion champ)
    {
        playerID = id -1;
        totalScore = 0;
        multiplier = 1;
        champion = champ;
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
        timer = 0.0f;
    }

    public void IncreaseMultiplier()
    {
        multiplier = Mathf.Min(multiplier + 1, 4);
        timer = 0.0f;
    }
    public void AddScore(int amount)
    {
        if (ScoreManager.GetInstance().gameStart)
        {
            if (amount < 0)
            {
                ResetMultiplier();
            }
            totalScore = Mathf.Min(Mathf.Max(0, totalScore + amount * multiplier), 99999999);
        }
    }

    public int TotalScore
    {
        get
        {
            return totalScore;
        }

    }
     public override bool Equals(object obj)
    {
        if (obj == null) { return false ; }
        Score s = obj as Score;
        if (s == null) { return false; }
        else return Equals(s);
    }

    public bool Equals(Score other)
    {
        if (other == null) return false;
        return (this.totalScore.Equals(other.totalScore));
    }

    public int CompareTo(Score comparePart)
    {
        if (comparePart == null)
            return 1;

        else 
            return comparePart.totalScore.CompareTo(this.totalScore);   //inverse sorting - greater to smaller value
    }

    public override int GetHashCode()
    {
        return totalScore;
    }
}

[Serializable]
public struct LeaderboardData
{
    public List<ScoreData> leaderboard;

    public LeaderboardData(List<ScoreData> l)
    {
        leaderboard = l;
    }
}

[Serializable]
public class ScoreData : IEquatable<ScoreData>, IComparable<ScoreData>
{
    public string playerName;
    public int totalScore;
    public Enum_Champion champion;

    public ScoreData(int score, Enum_Champion champ, string p = "AAA")
    {
        playerName = p;
        totalScore = score;
        champion = champ;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) { return false ; }
        ScoreData s = obj as ScoreData;
        if (s == null) { return false; }
        else return Equals(s);
    }

    public bool Equals(ScoreData other)
    {
        if (other == null) return false;
        return (this.totalScore.Equals(other.totalScore));
    }

    public int CompareTo(ScoreData comparePart)
    {
        if (comparePart == null)
            return 1;

        else 
            return comparePart.totalScore.CompareTo(this.totalScore);   //inverse sorting - greater to smaller value
    }

    public override int GetHashCode()
    {
        return totalScore;
    }
}
[Serializable]
public enum Enum_Champion
{
    Knight,
    Sorcerer,
    Archer
}