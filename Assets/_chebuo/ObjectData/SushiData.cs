using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SushiData", menuName = "Scriptable Objects/SushiData")]
public class SushiData : ScriptableObject
{
    public List<SushiList> sushiLists;
}

[System.Serializable]
public class SushiList
{
    public GameObject sushi;
}
