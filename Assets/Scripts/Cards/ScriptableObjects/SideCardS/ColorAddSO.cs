using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New ColorAddSO")]
public class ColorAddSO : SideCardSO, ICardDecorator
{
    public Colors Color;

    public override bool Copyable => false;

    public HandCardData Decorate(object Caller, HandCardData card)
    {
        card.cardColors.Add(Color);

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
