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
    [SerializeField] Transform enemySlot1;

    public CardController selectedCard;
    public CardController attackingCard;
    public bool isPlayerTurn = true;

 
    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        CreateCard(3, playerHand, true);
        CreateCard(2, playerHand, true);
        CreateCard(1, playerHand, true);
        CreateCard(0, playerHand, true);

        CreateCard(3, enemyHand, false);
        CreateCard(2, enemyHand, false);
        CreateCard(1, enemyHand, false);
        CreateCard(0, enemyHand, false);
        CreateCard(2, enemySlot1, false, true);
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if(isPlayerTurn)
        {
            foreach(CardController card in FindObjectsOfType<CardController>())
            {
                if(card.isPlayerCard == isPlayerTurn && card.isInField)
                {
                    card.canAttack = true;

                    Debug.Log(card.model.name+"が攻撃可能");
                }
            }
            Debug.Log("PlayerTurn");
        }
        else
        {
            Debug.Log("EnemyTurn");
        }
    }

    // カードを生成するメソッド
    void CreateCard(int cardId, Transform trans, bool isPlayer, bool inField = false)
    {
        // cardPrefabをtransに生成する
        CardController card = Instantiate(cardPrefab, trans);

        card.Init(cardId);

        card.isPlayerCard = isPlayer;

        card.isInField = inField;
    }
}
