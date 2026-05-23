
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
            if(isPlayerCard)
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

        if(manager.isPlayerTurn == false)
        {
            return;
        }
        if(!isPlayerCard)
        {
            return;
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
        Debug.Log(attacker.model.name + " → " + model.name);

        Destroy(attacker.gameObject);

        Destroy(gameObject);
    }
}
