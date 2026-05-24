using UnityEngine;
using UnityEngine.EventSystems;

public class EndlessButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ショップマネージャー")]
    public EndlessShopManager shopManager;

    [Header("このボタンの寿司番号 (0, 1, 2...)")]
    public int sushiIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopManager != null) shopManager.ShowSushiSpecs(sushiIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopManager != null) shopManager.HideSushiSpecs();
    }
}