using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CardPieceSO")]
public class CardPieceSO : HandCardSO, IObserverOnStartTurn
{
    public int number;

    public override bool overrideValue => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnTurnStart, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibraryLater(DictionaryTypes.OnTurnStart, subscriber);
    }


    public IEnumerator TriggerOnStartTurn(EventDataArgs args)
    {
        EntityHandler entity = args.Data as EntityHandler;
        HandCardDataHandler card = args.Sender as HandCardDataHandler;


        HandCardDataHandler[] pieces = entity.Hand.Where(o => o.data is CardPieceSO).ToArray();

        HandCardDataHandler one = null, two = null, three = null, four = null;

        foreach (var item in pieces)
        {
            switch ((item.data as CardPieceSO).number)
            {
                case 1: one = item; break;
                case 2: two = item; break;
                case 3: three = item; break;
                case 4: four = item; break;
            }
        }
        yield return entity.TriggerCard(one);
        yield return entity.TriggerCard(two);
        yield return entity.TriggerCard(three);
        yield return entity.TriggerCard(four);

        HandCardSO TheOne = GameManager.instance.AssetLibrary.FetchHandCard("Mau, the Prohibited One");

        HandCardDataHandler Mau = new HandCardDataHandler(TheOne, entity, false);
        Mau.InitForRound(entity.IsPlayer);
        Mau.RevealCard();

        entity.currDeck.RemoveCard(one, two, three, four);
        entity.visuals.RemoveFromHand(one.visuals, two.visuals, three.visuals, four.visuals);
        entity.Hand.Remove(one);
        entity.Hand.Remove(two);
        entity.Hand.Remove(three);
        entity.Hand.Remove(four);


        one.temp = true;
        two.temp = true;
        three.temp = true;
        four.temp = true;

        yield return entity.AddHandCard(Mau);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Data as EntityHandler;
        HandCardDataHandler one = null, two = null, three = null, four = null;
        HandCardDataHandler[] pieces = entity.Hand.Where(o => o.data is CardPieceSO).ToArray();

        foreach (var item in pieces)
        {
            switch ((item.data as CardPieceSO).number)
            {
                case 1: one = item; break;
                case 2: two = item; break;
                case 3: three = item; break;
                case 4: four = item; break;
            }
        }
        return entity == card.owner && base.CanTrigger(card, args, EventType) & one != null && two != null && three != null && four != null;
    }
}
