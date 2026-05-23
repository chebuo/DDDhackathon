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

        manager.selectedCard.transform.SetParent(transform);

        manager.selectedCard.isInField = true;

        manager.selectedCard.canAttack = false;

        manager.selectedCard.transform.localPosition = Vector3.zero;

        manager.selectedCard.Deselect();

        manager.selectedCard = null;
    }
}