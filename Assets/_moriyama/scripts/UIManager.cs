using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{

    [SerializeField] CardController cardPrefab; // カードプレハブ
    
    // カードの生成場所
    [SerializeField] Transform playerHand;
    [SerializeField] Transform playerField;
    [SerializeField] Transform enemyHand;
    [SerializeField] Transform enemyField;
    [SerializeField] Transform enemySlot1;
    [SerializeField] TMPro.TextMeshProUGUI playerHPText;
    [SerializeField] TMPro.TextMeshProUGUI enemyHPText;
    [SerializeField] TMPro.TextMeshProUGUI playerCostText;
    [SerializeField] TMPro.TextMeshProUGUI enemyCostText;

    public CardController selectedCard;
    public CardController attackingCard;
    public bool isPlayerTurn = true;
    public int playerHP = 10000;
    public int enemyHP = 10000;

    public int playerMaxCost = 1000;
    public int playerCurrentCost = 1000;
    public int playerCarryOverCost = 0;

    public int enemyMaxCost = 1000;
    public int enemyCurrentCost = 1000;
    public int enemyCarryOverCost = 0;
    public static bool result = true;

    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        CreateCard(12, playerHand, true);
        CreateCard(5, playerHand, true);
        CreateCard(19, playerHand, true);
        CreateCard(12, playerHand, true);
        CreateCard(3, playerHand, true);
        CreateCard(2, playerHand, true);
        CreateCard(1, playerHand, true);
        CreateCard(0, playerHand, true);

        CreateCard(11, enemyHand, false);
        CreateCard(6, enemyHand, false);
        CreateCard(9, enemyHand, false);
        CreateCard(17, enemyHand, false);
        CreateCard(3, enemyHand, false);
        CreateCard(2, enemyHand, false);
        CreateCard(1, enemyHand, false);
        CreateCard(0, enemyHand, false);

        UpdateHPUI();
        UpdateCostUI();
    }

    public void EndTurn()
{
    if(isPlayerTurn)
    {
        int playerRemain = Mathf.Min(playerCurrentCost, playerMaxCost);

        playerCarryOverCost = Mathf.Min((playerRemain / 2 / 1000) * 1000, 2000);
    }
    else
    {
        int enemyRemain = Mathf.Min( enemyCurrentCost, enemyMaxCost);

        enemyCarryOverCost = Mathf.Min((enemyRemain / 2 / 1000)* 1000, 2000);
    }

    isPlayerTurn = !isPlayerTurn;

    if(isPlayerTurn)
    {
        playerMaxCost += 1000;

        playerCurrentCost = playerMaxCost + playerCarryOverCost;
    }
    else
    {
        enemyMaxCost += 1000;

        enemyCurrentCost = enemyMaxCost + enemyCarryOverCost;
    }

    UpdateCostUI();

    foreach(CardController card in FindObjectsOfType<CardController>())
    {
        if(card.isPlayerCard == isPlayerTurn && card.isInField)
        {
            card.canAttack = true;
        }
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
    public void AttackLeader(bool attackPlayer, int damage)
    {
        if(attackPlayer)
        {
            playerHP -= damage;

            UpdateHPUI();

            Debug.Log("Player HP : " + playerHP);

            if(playerHP <= 0)
            {
                result = true;
                SceneManager.LoadScene("result");
                Debug.Log("Enemy Win");
            }
        }
        else
        {
            enemyHP -= damage;

            UpdateHPUI();

            Debug.Log("Enemy HP : " + enemyHP);

            if(enemyHP <= 0)
            {
                result = false;
                SceneManager.LoadScene("result");
                Debug.Log("Player Win");
            }
        }
    }
    void UpdateHPUI()
    {
        playerHPText.text = "HP : " + playerHP;
        enemyHPText.text = "HP : " + enemyHP;
    }
    public void UpdateCostUI()
    {
        playerCostText.text = playerCurrentCost + " / " + playerMaxCost;
        enemyCostText.text = enemyCurrentCost + " / " + enemyMaxCost;
    }
}
