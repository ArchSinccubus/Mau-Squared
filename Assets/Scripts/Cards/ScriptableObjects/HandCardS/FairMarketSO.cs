using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New FairMarketSO")]
public class FairMarketSO : MagpieSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        CoroutineWithData<long> cd = new CoroutineWithData<long>(GameManager.Round, GetDeckCard(card.owner));

        yield return cd.coroutine;

        while (cd.result == 0)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result != 0)
        {
            HandCardDataHandler newCard = card.owner.currDeck.FetchCard(cd.result);

            newCard.owner = target;

            if (target.IsPlayer)
            {
                newCard.PlayerControl = true;
                newCard.visuals.RevealCard();
            }
            else
            {
                newCard.PlayerControl = false;
                newCard.visuals.HideCard();
            }

            yield return target.AddHandCard(newCard);

            int loss = target.CalculateLoss(MoneyAmount);

            yield return target.ReduceMoney(loss);
            yield return card.owner.AddMoney(loss);
        }
    }
}
