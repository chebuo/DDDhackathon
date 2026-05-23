using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 
public class CardModel
{
    public int cardId;
    public string name;
    //public int cost;
    public int power;
    public string description;
    public Sprite cardImage;
 
    public CardModel(int cardID) // データを受け取り、その処理
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card" + cardID);
 
        cardId = cardEntity.cardId;
        name = cardEntity.name;
        //cost = cardEntity.cost;
        power = cardEntity.power;
        cardImage = cardEntity.cardImage;
        description = cardEntity.description;
    }
}