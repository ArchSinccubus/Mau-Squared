using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New ImaginarySO")]
public class ImaginarySO : SideCardSO, ICardDecorator
{
    public override bool Copyable => false;

    public HandCardData Decorate(object Caller, HandCardData card)
    {
        if (card.cardValues.Contains(1))
        {
            card.PreWild = true;
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
