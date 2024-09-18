using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Value Adder", menuName = "Mau/Cards/Side/New ValueAdderSO")]
public class ValueAdderCardSO : SideCardSO, ICardDecorator
{
    public List<int> values;

    public override bool Copyable => false;

    public HandCardData Decorate(object Caller, HandCardData card)
    {
        card.cardValues.AddRange(values);

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
