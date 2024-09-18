using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HandSmokeCardSO")]
public class HandSmokeCardSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();

        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(target.Hand.Where(c => !c.Smoked).ToList(), NumberAmount);

        yield return target.SmokeCards(card.owner, cardsToSmoke.ToArray());
    }
}
