using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShopCardContainer : CardContainer
{
    public void OnCardSelect(int price, ICardVisuals vis)
    {
        (slots[vis] as ShopCardSlotHandler).RevealPriceTag(price); 
    }

    public void OnCardDeselect(ICardVisuals vis) 
    {
        //(slots[vis] as ShopCardSlotHandler).HidePriceTag();
    }
}
