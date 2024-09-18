using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New PrimeSO")]
public class PrimeSO : SideCardSO, ICardDecorator
{
    public HandCardData Decorate(object Caller, HandCardData card)
    {
        if (ContainsPrime(card.cardValues))
        {
            card.Mult *= MultAmount;
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

    public bool ContainsPrime(List<int> nums)
    {
        foreach (var item in nums)
        {
            if (item == 1 || item == 2 || item == 3 || item == 5 || item == 7)
            {
                return true;
            }
        }

        return false;
    }
}
