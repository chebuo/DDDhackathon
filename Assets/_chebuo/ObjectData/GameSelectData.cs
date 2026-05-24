using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameSelectData", menuName = "Scriptable Objects/GameSelectData")]
public class GameSelectData : ScriptableObject
{
    public List<GameList> games=new List<GameList>();
}

[System.Serializable]
public class GameList
{
    public string gameScene;
    public Sprite gameIcon;
    public string gameName;
}
