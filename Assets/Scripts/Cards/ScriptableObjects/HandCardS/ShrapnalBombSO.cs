using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New ShrapnalBombSO")]
public class ShrapnalBombSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();
        CoroutineWaitForList list = new CoroutineWaitForList();

        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        int cardsInHand = target.Hand.Count;

        cardsToSmoke = target.Hand;


        yield return target.SmokeCards(card.owner, cardsToSmoke.ToArray());


        yield return target.RecycleCards(target.Hand.ToArray());
    }
}
