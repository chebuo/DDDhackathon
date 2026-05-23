using UnityEngine;
using UnityEngine.EventSystems; // ★マウスイベントを検知するのに必要

// IPointerEnterHandler, IPointerExitHandler をつけることで検知可能になります
public class SushiButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("デッキマネージャー")]
    public DeckManager deckManager;

    [Header("このボタンの寿司番号 (0, 1, 2...)")]
    public int sushiIndex;

    // マウスカーソルがボタンに乗った瞬間
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (deckManager != null)
        {
            deckManager.ShowSushiSpecs(sushiIndex);
        }
    }

    // マウスカーソルがボタンから外れた瞬間
    public void OnPointerExit(PointerEventData eventData)
    {
        if (deckManager != null)
        {
            deckManager.HideSushiSpecs();
        }
    }
}