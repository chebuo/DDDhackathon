
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class CardController : MonoBehaviour, IPointerClickHandler
{
    public CardView view; // カードの見た目の処理
    public CardModel model; // カードのデータを処理

    public bool isInField = false;
    Vector3 defaultScale;
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
        if(isInField)
        {
            return;
        }

        UIManager manager = FindObjectOfType<UIManager>();

        if(manager.selectedCard != null)
        {
            manager.selectedCard.Deselect();
        }
        
        manager.selectedCard = this;

        Select();
    }
}
