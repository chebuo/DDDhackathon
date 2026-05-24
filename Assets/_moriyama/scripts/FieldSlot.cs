using UnityEngine;
using UnityEngine.EventSystems;

public class FieldSlot :MonoBehaviour, IPointerClickHandler
{
    public bool isPlayerField;
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager manager = FindObjectOfType<UIManager>();

        if (manager.selectedCard == null)
        {
            return;
        }
        if(manager.selectedCard.isPlayerCard != isPlayerField)
        {
            return;
        }
        if(manager.selectedCard.isPlayerCard != manager.isPlayerTurn)
        {
            return;
        }

        if(manager.selectedCard.isPlayerCard)
        {
            if(manager.playerCurrentCost < manager.selectedCard.model.cost)
            {
                return;
            }
        }
        else
        {
            if(manager.enemyCurrentCost < manager.selectedCard.model.cost)
            {
                return;
            }
        }
        

        manager.selectedCard.transform.SetParent(transform);

        manager.selectedCard.isInField = true;

        manager.selectedCard.OnSummon();

        manager.selectedCard.canAttack = false;

        manager.selectedCard.transform.localPosition = Vector3.zero;

        if(manager.selectedCard.isPlayerCard)
        {
            manager.playerCurrentCost -= manager.selectedCard.model.cost;
            Debug.Log(manager.playerCurrentCost);
        }
        else
        {
            manager.enemyCurrentCost -= manager.selectedCard.model.cost;
            Debug.Log(manager.enemyCurrentCost);
        }
        manager.UpdateCostUI();
        manager.selectedCard.Deselect();

        manager.selectedCard = null;
    }
}