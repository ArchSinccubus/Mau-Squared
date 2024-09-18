using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardSlotHandler : CardSlotHandler
{
    public Image TagImage;
    public TextMeshProUGUI PriceTag;
    public Image LockImage;

    public override void init(ICardVisuals newCard, Transform T, CardContainer parent)
    {
        base.init(newCard, T, parent);

        TagImage.transform.SetAsFirstSibling();
        LockImage.transform.SetAsFirstSibling();
    }

    public void RevealPriceTag(int price)
    { 
        TagImage.gameObject.SetActive(true);
        PriceTag.text = price.ToString();
    }

    public void HidePriceTag()
    {
        PriceTag.text = "";
        TagImage.gameObject.SetActive(false);
    }

    public void ChangeLockState(bool state) 
    {
        LockImage.gameObject.SetActive(state);
    }
}
