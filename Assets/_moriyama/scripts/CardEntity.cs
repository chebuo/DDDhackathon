using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(fileName = "CardEntity", menuName = "Create CardEntity")]
 
public class CardEntity : ScriptableObject
{
    public int cardId;
    public new string name;
    public int cost;
    public int power;
    public int effectID;
    public bool isGuard;
    public string description;
    public Sprite cardImage;
}