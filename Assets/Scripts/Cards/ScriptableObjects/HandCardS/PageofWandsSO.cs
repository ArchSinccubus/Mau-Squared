using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PageofWandsSO")]
public class PageofWandsSO : HandCardSO
{
    public override bool ChoiceCard => true;

    public override bool Tarot => true;

    public string choiceText;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler> cd = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, GetPileCard(card.owner));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        HandCardDataHandler newCard = new HandCardDataHandler(cd.result);
        newCard.InitForRound(card.owner.IsPlayer);
        newCard.temp = true;

        yield return card.owner.AddHandCard(newCard);
    }

    public IEnumerator GetPileCard(EntityHandler entity)
    {
        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        list = GameManager.currRun.RoundRand.GetRandomElements(GameManager.Round.Pile, ChoiceAmount);
            
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.ToArray(), choiceText, 1, CardMenuChoiceMode.Open, SetChoiceType.HandAdd));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return cd.result[0];
        }
    }
}
