using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CarWashSO")]
public class CarWashSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {

        yield return card.owner.CleanCards(card.owner.currDeck.DeckBase.ToArray());

    }
}
