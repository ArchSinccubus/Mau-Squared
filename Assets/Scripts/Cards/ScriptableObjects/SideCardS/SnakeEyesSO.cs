using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New SnakeEyesSO")]
public class SnakeEyesSO : SideCardSO, ICardDecorator
{
    public HandCardData Decorate(object Caller, HandCardData card)
    {
        if (card.cardValues.Count > 0)
        {
            if (card.cardValues[0] % 2 != 0)
            {
                card.Mult *= MultAmount;
            }

        }      

        return card;
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.AddCardDecorator(this, card);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.RemoveCardDecorator(card);

        base.OnRemove(card);
    }


}
