using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New ColorScoreAdderSO")]
public class ColorScoreAdderSO : SideCardSO, ICardDecorator
{
    public Colors c;

    public HandCardData Decorate(object caller, HandCardData card)
    {
        if (card.cardColors.Contains(c))
        {
            card.Score += ScoreAmount;
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
