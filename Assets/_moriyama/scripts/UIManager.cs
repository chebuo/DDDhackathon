using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIManager : MonoBehaviour
{

    [SerializeField] CardController cardPrefab; // カードプレハブ
    
    // カードの生成場所
    [SerializeField] Transform playerHand;
    [SerializeField] Transform playerField;
    [SerializeField] Transform enemyHand;
    [SerializeField] Transform enemyField;

    public CardController selectedCard;
    public bool isPlayerTurn = true;

 
    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        CreateCard(3, playerHand);
        CreateCard(2, playerHand);
        CreateCard(1, playerHand);
        CreateCard(0, playerHand);
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if(isPlayerTurn)
        {
            Debug.Log("PlayerTurn");
        }
        else
        {
            Debug.Log("EnemyTurn");
        }
    }

    // カードを生成するメソッド
    void CreateCard(int cardId, Transform trans)
    {
        // cardPrefabをtransに生成する
        CardController card = Instantiate(cardPrefab, trans);
        card.Init(cardId);
    }
}
