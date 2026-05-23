using UnityEngine;
using UnityEngine.EventSystems;

public class FieldSlot :MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager manager = FindObjectOfType<UIManager>();

        if (manager.selectedCard == null)
        {
            return;
        }

        manager.selectedCard.transform.SetParent(transform);

        manager.selectedCard.isInField = true;

        manager.selectedCard.transform.localPosition = Vector3.zero;

        manager.selectedCard.Deselect();

        manager.selectedCard = null;
    }
}