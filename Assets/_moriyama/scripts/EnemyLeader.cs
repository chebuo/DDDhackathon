using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyLeader :MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager manager = FindObjectOfType<UIManager>();

        if(manager.attackingCard == null)
        {
            return;
        }

        if(!manager.attackingCard.canAttack)
        {
            return;
        }

        if(manager.HasGuard(false))
        {
            return;
        }

        manager.AttackLeader(false, manager.attackingCard.model.currentPower);

        manager.attackingCard.canAttack = false;

        manager.attackingCard = null;
    }
}
