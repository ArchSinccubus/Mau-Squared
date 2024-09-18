using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New TradeOfferCardSO")]
public class TradeOfferCardSO : HandChoiceBaseSO
{
    public override bool ChoiceCard => true;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Open, null, HandChoiceType.Other));
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        CoroutineWaitForList list = new CoroutineWaitForList();

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            HandCardDataHandler OpCard = target.Hand[GameManager.currRun.RoundRand.NextInt(0, target.Hand.Count)];

            yield return list.CountCoroutine(ExchangeCard(card.owner, target, cd.result[0]));
            yield return list.CountCoroutine(ExchangeCard(target, card.owner, OpCard));
        }

        yield return list;
    }

    public IEnumerator ExchangeCard(EntityHandler origOwner, EntityHandler newOwner, HandCardDataHandler card)
    {

        

        Coroutine move = GameManager.instance.StartCoroutine(newOwner.AddHandCard(card));
        origOwner.Hand.Remove(card);
        yield return new WaitForGameSeconds(0.1f);
        origOwner.visuals.RemoveFromHand(card.visuals);

        yield return move; 
        card.owner = newOwner;

        card.PlayerControl = newOwner.IsPlayer;
        if (newOwner.IsPlayer)
        {
            card.visuals.EnableCardForPlayer();
        }
        else
            card.visuals.DisableCardForPlayer();

    }
}
