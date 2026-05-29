using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class CardController : MonoBehaviour, IPointerClickHandler
{
    public CardView view; // カードの見た目の処理
    public CardModel model; // カードのデータを処理

    Vector3 defaultScale;
    public bool isPlayerCard;//true=player false=Enemy
    public bool isInField = false;
    public bool canAttack = false;
    bool isSelected = false;
    

    private void Start()
    {
        defaultScale = transform.localScale;
    }
    private void Awake()
    {
        view = GetComponent<CardView>();
    }
    public void Init(int cardID) // カードを生成した時に呼ばれる関数
    {
        model = new CardModel(cardID); // カードデータを生成
        view.Show(model); // 表示
    }

    public void Select()
    {
        isSelected = true;

        transform.localScale = defaultScale * 1.2f;
    }
    public void Deselect()
    {
        isSelected = false;

        transform.localScale = defaultScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager manager = FindObjectOfType<UIManager>();
        
        if(isInField)
        {
            if(isPlayerCard == manager.isPlayerTurn)
            {
                if(!canAttack)
                {
                    return;
                }

                manager.attackingCard = this;

                Debug.Log(model.name+"が攻撃選択状態");

                return;
            }
            else
            {
                if(manager.attackingCard == null)
                {
                    return;
                }

                Battle(manager.attackingCard);

                manager.attackingCard = null;

                return;
            }
        }

        if(manager.selectedCard != null)
        {
            manager.selectedCard.Deselect();
        }
        
        manager.selectedCard = this;

        Select();
    }
    void Battle(CardController attacker)
    {
        int attackerPower = attacker.model.currentPower;
        int defenderPower = model.currentPower;

        Debug.Log(attacker.model.name +"(" + attackerPower + ")" + " vs " + 
         model.name + "(" + defenderPower + ")");

        if(attackerPower > defenderPower)
        {
            attacker.model.currentPower -= defenderPower;
            attacker.view.Refresh(attacker.model);
            Destroy(gameObject);
            attacker.canAttack = false;
        }
        else if(attackerPower < defenderPower)
        {
            model.currentPower -= attackerPower;
            view.Refresh(model);
            Destroy(attacker.gameObject);
        }
        else
        {
            Destroy(gameObject);
            Destroy(attacker.gameObject);
        }

        
    }
    public void OnSummon()
    {
        if(model.effectID == 1)
        {
            DamageEnemyCard(1000);
        }
    }
    void DamageEnemyCard(int damage)
    {
        foreach(CardController card in FindObjectsOfType<CardController>())
        {
            if(card.isPlayerCard != isPlayerCard && card.isInField)
            {
                card.model.currentPower -= damage;

                card.view.Refresh(card.model);

                Debug.Log(card.model.name + "に" + damage + "ダメージ");

                if(card.model.currentPower <= 0)
                {
                    Destroy(card.gameObject);
                }

                return;
            }
        }
    }
}
