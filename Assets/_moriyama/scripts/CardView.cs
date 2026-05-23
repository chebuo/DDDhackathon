
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class CardView : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image iconImage;
 
    public void Show(CardModel cardModel) // cardModelのデータ取得と反映
    {
        nameText.text = cardModel.name;
        iconImage.sprite = cardModel.cardImage;
        //costText.text = cardModel.cost.ToString();
        powerText.text = cardModel.power.ToString();
        descriptionText.text = cardModel.description;
    }
}
