using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameSelectData", menuName = "Scriptable Objects/GameSelectData")]
public class GameSelectData : ScriptableObject
{
    public List<GameList> games=new List<GameList>();
    public string playerName;

    public string GetScoreHMS(float score)
    {
        int hours = Mathf.FloorToInt(score / 3600);
        int minutes = Mathf.FloorToInt(score / 60) % 60;
        int seconds = Mathf.FloorToInt(score % 60);
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    public string GetScoreMSF(float score)
    {
        int minutes = Mathf.FloorToInt(score / 60) % 60;
        int seconds = Mathf.FloorToInt(score % 60);
        int millis=Mathf.FloorToInt((score - Mathf.Floor(score)) * 100);
        return $"{minutes:00}:{seconds:00}:{millis:00}";
    }
}

[System.Serializable]
public class GameList
{
    public string gameScene;
    public Sprite gameIcon;
    public string gameName;
    public string score;
    public string scoreName;
}


