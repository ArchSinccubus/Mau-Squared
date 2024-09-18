using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New LightPollutionSO")]
public class LightPollutionSO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Open, o => o.Smoked, HandChoiceType.Clean));

        yield return cd.coroutine;
        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return card.owner.CleanCards(cd.result);

        HandCardDataHandler deckCard = GameManager.currRun.RoundRand.GetRandomElements(target.currDeck.DeckBase.Where(c => !c.Smoked).ToList(), NumberAmount)[0];
        HandCardDataHandler handCard = GameManager.currRun.RoundRand.GetRandomElements(target.Hand.Where(c => !c.Smoked).ToList(), NumberAmount)[0];

        yield return target.SmokeCards(card.owner, deckCard, handCard);
    }
}
