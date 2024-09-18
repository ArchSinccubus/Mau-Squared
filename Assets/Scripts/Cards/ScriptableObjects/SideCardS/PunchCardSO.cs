using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New PunchCardSO")]
public class PunchCardSO : SideCardSO, ICardDecorator
{
    public HandCardData Decorate(object caller, HandCardData card)
    {
        EntityHandler owner = (caller as SideCardDataHandler).owner;

        float finalMult = 1;

        foreach (SideCardDataHandler item in owner.SideCards)
        {
            if (item.data.Clickable && item.Used)
            {
                finalMult += MultAmount;
            }
        }

        card.Mult *= finalMult;

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
