using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New WaterBaloonSO")]
public class WaterBaloonSO : DeckSmokePlaySO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();

        int amount = GameManager.Round.Pile.Where(o => o.returnUnmodifiedData().cardColors.Contains(Colors.Blue)).Count();

        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        int cardsInHand = target.currDeck.DeckBase.Count;

        cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(target.currDeck.DeckBase.Where(c => !c.Smoked).ToList(), NumberAmount);

        yield return SmokeCards(card, target, cardsToSmoke.ToArray());

    }
}
