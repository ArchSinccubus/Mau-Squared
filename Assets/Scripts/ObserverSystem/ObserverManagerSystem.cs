using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The general Observer event system!
/// This is how it works: This is a static class that contains all the events the game needs. You need a new one? Just add a new Enum entry. Then, in the cardSO children, override the subscribe feature and call
/// the subscription method as needed, with the appropriate enum. This can easily scale upwards to whatever events are required, and cards can subscribe to several events at once. The reason it's static is because
/// There's no need for every single class that needs to fire events to have its own container of observers. Plus, this way everything is decoupled.
/// </summary>

public class ObserverManagerSystem
{
    public static Dictionary<DictionaryTypes, List<EventDataStruct>> CardEventLibrary;

    static Dictionary<ISubscriber, DictionaryTypes> UnsubLaterList;

    static List<IEnumerator> ShopDeckEvents;

    public static HandCardDataHandler playedCard;

    public static void InitLibrary()
    {
        UnsubLaterList = new Dictionary<ISubscriber, DictionaryTypes>();

        if (CardEventLibrary == null)
        {
            CardEventLibrary = new Dictionary<DictionaryTypes, List<EventDataStruct>>();
        }
        else
        {
            CardEventLibrary.Clear();
        }

        foreach (DictionaryTypes item in Enum.GetValues(typeof(DictionaryTypes)))
        {
            if (!CardEventLibrary.ContainsKey(item))
            {
                CardEventLibrary.Add(item,new List<EventDataStruct>());
            }
        }
    }

    public static void UnsubscribeFromLibrary(DictionaryTypes Library, object card)
    {
        if (CardEventLibrary.ContainsKey(Library))
        {
            CardEventLibrary[Library].RemoveAll(o => o.Value == card);
        }
    }

    public static void UnsubscribeFromLibraryLater(DictionaryTypes Library, object card)
    {
        UnsubLaterList.Add(card as ISubscriber, Library);

    }

    public static void SubscribeToLibrary(DictionaryTypes Library, object card, ISubscriber subscriber)
    {
        if (CardEventLibrary.ContainsKey(Library))
        {
            CardEventLibrary[Library].Add(new EventDataStruct() { Value = subscriber, Key = card });
        }
    }

    public static void ShiftEventPosition(DictionaryTypes Library, ISubscriber subscriber, EventShiftOptions mode)
    {
        int index = 0;

        if (CardEventLibrary.ContainsKey(Library))
        {
            if (CardEventLibrary[Library].Where(o => o.Value == subscriber).Count() > 0)
            {
                index = CardEventLibrary[Library].FindIndex(o => o.Key == subscriber);
            }
        }

        EventDataStruct data = CardEventLibrary[Library][index];
        CardEventLibrary[Library].RemoveAt(index);

        switch (mode)
        {
            case EventShiftOptions.First:
                index = 0;
                break;
            case EventShiftOptions.Up:
                if (index < CardEventLibrary[Library].Count - 1)
                {
                    index++;
                }
                break;
            case EventShiftOptions.Down:
                if (index > 0)
                {
                    index--;
                }
                break;
            case EventShiftOptions.End:
                index = CardEventLibrary[Library].Count - 1;
                break;
            default:
                break;
        }

        CardEventLibrary[Library].Insert(index, data);
    }

    public static IEnumerator NotifyEvents(DictionaryTypes events, object caller, object data)
    {
        bool ShopTrigger = GameManager.currRun.runState == GameState.InShop;
        bool ShowDeck = false;
        bool ShowSide = false;

        if (CardEventLibrary.ContainsKey(events))
        {
            foreach (var item in CardEventLibrary[events])
            {
                bool triggered = false;
                EventDataArgs args = new EventDataArgs() { Caller = caller, Data= data, Sender = item.Key};

                if (args.Sender is HandCardDataHandler)
                {
                    if (ShopTrigger && !ShowDeck)
                    {
                        ShowDeck = true;
                        yield return MoveShopDeck(true);
                    }

                }
                else if (args.Sender is SideCardDataHandler)
                {
                    if (ShopTrigger && !ShowSide)
                    {
                        ShowDeck = true;
                        yield return MoveShopSide(true);
                    }
                }

                BaseCardDataHandler card = null;

                if (item.Key is BaseCardDataHandler)
                {
                    card = item.Key as BaseCardDataHandler;
                }
                else
                {
                    triggered = true;
                }

                if (card.baseData.CanTrigger(card, args, events))
                {
                    if (!card.baseData.SilentTrigger && !card.baseData.ChoiceCard)
                    {
                        yield return card.owner.TriggerCard(card, card.baseData.ReturnTriggeredText());
                    }
                    triggered = true;
                }

                if (triggered)
                {
                    switch (events)
                    {
                        case DictionaryTypes.OnTurnStart:
                            yield return (item.Value as IObserverOnStartTurn).TriggerOnStartTurn(args);
                            break;
                        case DictionaryTypes.OnTurnEnd:
                            yield return (item.Value as IObserverOnEndTurn).TriggerOnEndTurn(args);
                            break;
                        case DictionaryTypes.OnCardPlayed:
                            args.Data = playedCard;
                            if (playedCard != item.Key)
                            {
                                yield return (item.Value as IObserverOnCardPlayed).TriggerOnCardPlayed(args);
                            }
                            break;
                        case DictionaryTypes.OnCardPlaced:
                            args.Data = playedCard;
                            yield return (item.Value as IObserverOnCardPlaced).TriggerOnCardPlaced(args);
                            break;
                        case DictionaryTypes.OnSmokedPlayed:
                            args.Data = playedCard;
                            yield return (item.Value as IObserverOnSmokedPlayed).TriggerOnSmokedPlayed(args);
                            break;
                        case DictionaryTypes.OnPreRoundStart:
                            yield return (item.Value as IObserverOnPreStartRound).TriggerPreStartRound(args);
                            break;
                        case DictionaryTypes.OnRoundStart:
                            yield return (item.Value as IObserverOnStartRound).TriggerStartRound(args);
                            break;
                        case DictionaryTypes.OnRoundEnd:
                            yield return (item.Value as IObserverOnEndRound).TriggerOnEndRound(args);
                            break;
                        case DictionaryTypes.OnRecycle:
                            yield return (item.Value as IObserverOnRecycle).TriggerOnRecycle(args);
                            break;
                        case DictionaryTypes.OnCardSmoked:
                            yield return (item.Value as IObserverOnCardSmoked).TriggerOnSmoked(args);
                            break;
                        case DictionaryTypes.OnCardCleared:
                            yield return (item.Value as IObserverOnCardCleaned).TriggerOnCleaned(args);
                            break;
                        case DictionaryTypes.OnDraw:
                            yield return (item.Value as IObserverOnDraw).TriggerOnDraw(args);
                            break;
                        case DictionaryTypes.OnPicked:
                            yield return (item.Value as IObserverOnPicked).TriggerOnPicked(args);
                            break;
                        case DictionaryTypes.OnBuy:
                            yield return (item.Value as IObserverOnBuy).TriggerOnBuy(args);
                            //ShopDeckEvents.Add((item.Value as IObserverOnBuy).TriggerOnBuy(args));
                            break;
                        case DictionaryTypes.OnSellSide:
                            yield return (item.Value as IObserverOnSideSell).TriggerOnSideSell(args);
                            break;
                        case DictionaryTypes.OnRemove:
                            yield return (item.Value as IObserverOnRemove).TriggerOnRemove(args);
                            break;
                        case DictionaryTypes.OnShopRefresh:
                            yield return (item.Value as IObserverOnRefresh).TriggerOnRefresh(args);
                            break;
                        case DictionaryTypes.OnShopEnter:
                            yield return (item.Value as IObserverOnShopEnter).TriggerOnShopEnter(args);
                            break;
                        case DictionaryTypes.OnShopExit:
                            yield return (item.Value as IObserverOnShopExit).TriggerOnShopExit(args);
                            break;
                        case DictionaryTypes.OnScoreChanged:
                            yield return (item.Value as IObserverOnScoreChanged).TriggerOnScoreChanged(args);
                            break;
                        case DictionaryTypes.OnMoneyChanged:
                            yield return (item.Value as IObserverOnMoneyChanged).TriggerOnMoneyChanged(args);
                            break;
                        case DictionaryTypes.OnActivateSideCard:
                            yield return (item.Value as IObserverOnActivateSideCard).TriggerOnActivateSideCard(args);
                            break;
                        default:
                            break;
                    }

                    if (!card.baseData.SilentTrigger)
                    {
                        if (!card.baseData.ChoiceCard && item.Key is HandCardDataHandler)
                        {
                            yield return (item.Key as HandCardDataHandler).owner.FinishTriggerCard(item.Key as HandCardDataHandler);
                        }
                        else
                        {
                            yield return card.owner.TriggerCard(card, card.baseData.ReturnTriggeredText());
                        }
                    }
                    triggered = false;

                }
            }

            if (ShowDeck)
            {
                yield return MoveShopDeck(false);
            }
            if (ShowSide)
            {
                yield return MoveShopSide(false);

            }

            foreach (var item in UnsubLaterList)
            {
                UnsubscribeFromLibrary(item.Value, item.Key);
            }
        }
    }

    public static IEnumerator NotifyPlayedCard(HandCardDataHandler card)
    {
        playedCard = card;

        if (card.data.CanActivateEffect(card, "OnThisPlayed"))
        {
            if (!card.data.ChoiceCard)
            {
                yield return card.owner.TriggerCard(card, card.data.ReturnPlayedText(card));
                yield return card.data.OnThisPlayed(card);
                yield return card.owner.FinishTriggerCard(card);
            }

            else
            {
                if (card.owner.Hand.Count > 0)
                {
                    yield return card.data.OnThisPlayed(card);
                    yield return card.owner.TriggerCard(card, card.data.ReturnPlayedText(card));
                    yield return card.owner.FinishTriggerCard(card);
                }
            }
        }
    }

    public static IEnumerator NotifyPlayedCardLate(HandCardDataHandler card)
    {
        if (card.data.CanActivateEffect(card, "OnThisPlaced") && card.data.Transformer)
        {
            yield return card.owner.TriggerCard(card);
            yield return card.data.OnThisPlaced(card);
            yield return card.owner.FinishTriggerCard(card);
        }
    }

    public static IEnumerator NotifyDrawnCard(HandCardDataHandler card) 
    {
        if (card.data.CanActivateEffect(card, "OnThisDrawn"))
        {
            yield return card.owner.TriggerCard(card, card.data.ReturnDrawnText());
            yield return card.data.OnThisDrawn(card);
            yield return card.owner.FinishTriggerCard(card);
        }
    }

    static IEnumerator MoveShopDeck(bool move)
    {
        PlayerHandler player = GameManager.currRun.player;
        PlayerVisualManager visuals = player.visuals as PlayerVisualManager;

        yield return visuals.MoveDeck(move);

    }

    static IEnumerator MoveShopSide(bool move)
    {
        PlayerHandler player = GameManager.currRun.player;
        PlayerVisualManager visuals = player.visuals as PlayerVisualManager;

        yield return visuals.MoveSide(move);

    }
}

