using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CupsSO")]
public class CupsSO : DeckSmokeCardSO
{
    public override int BasePrice => 1;

    public override IEnumerator OnThisDrawn(HandCardDataHandler card)
    {
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();

        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        int cardsInHand = target.currDeck.DeckBase.Count;

        cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(target.Hand.Where(c => !c.Smoked).ToList(), NumberAmount);

        if (cardsToSmoke.Count > 0)
        {
            yield return card.owner.TriggerCard(card);
            yield return SmokeCards(card, target, cardsToSmoke.ToArray());
        }

    }
}
