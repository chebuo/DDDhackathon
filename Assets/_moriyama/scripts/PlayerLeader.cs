using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLeader :
    MonoBehaviour,
    IPointerClickHandler
{
    public void OnPointerClick(
        PointerEventData eventData)
    {
        UIManager manager =
            FindObjectOfType<UIManager>();

        if(manager.attackingCard == null)
        {
            return;
        }

        if(!manager.attackingCard.canAttack)
        {
            return;
        }

        manager.AttackLeader(
            true,
            manager.attackingCard.model.currentPower);

        manager.attackingCard.canAttack = false;

        manager.attackingCard = null;
    }
}