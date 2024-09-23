using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopLogicHandler : MonoBehaviour, IGameScreen
{
    public int currMoney {
        get => GameManager.currRun.baseData.PlayerMoney;

        set {
            GameManager.currRun.baseData.PlayerMoney = value;

            visuals.UpdateMoney(value, value >= RefreshCost, value >= RemoveCost, value >= SideCardCost);
        }
    }

    public ShopVisualHandler visuals;
    public ShopAudioHandler Saudio;

    public BaseCardDataHandler selectedCard;

    public List<HandCardDataHandler> handCards, LockedHandCards;

    public List<SideCardDataHandler> sideCards, LockedSideCards;

    public CardMenuHandler deckViewForRemove;

    public int TimesBought;

    public int TimesRefreshed;

    public int TimesRemoved;

    public int RefreshCost;
    public int RemoveCost;
    public int SideCardCost;

    public bool CanAct;
    public bool FreeRefresh;
    public bool FreeRemove;

    Transform IGameScreen.transform { get => transform; }
    public ScreenMoverHelper MoverHelper { get => visuals.mover; }

    public void InitScreen()
    {
        if (LockedHandCards == null)
        {
            LockedHandCards = new List<HandCardDataHandler>();
        }

        if (LockedSideCards == null)
        {
            LockedSideCards = new List<SideCardDataHandler>();
        }

        handCards = GenerateHandCards().ToList();
        sideCards = GenerateSideCards().ToList();

        RefreshCost = GameManager.currRun.shopData.RefreshCostStart;
        RemoveCost = GameManager.currRun.shopData.RemoveCostStart;
        SideCardCost = GameManager.currRun.shopData.AddSideCardSpaceCostStart;


        visuals.init(handCards.ToArray(), sideCards.ToArray(), SelectCard);
        visuals.UpdateRefreshPrice(RefreshCost);
        visuals.UpdateRemovePrice(RemoveCost);
        visuals.UpdateSidePrice(SideCardCost);

        foreach (BaseCardDataHandler item in LockedHandCards)
        {
            visuals.ChangeLockState(item.visuals, item.baseData.SideCard, true);
        }

        foreach (BaseCardDataHandler item in LockedSideCards)
        {
            visuals.ChangeLockState(item.visuals, item.baseData.SideCard, true);
        }

        GameManager.currRun.runState = GameState.InShop;

        CanAct = true;


        currMoney = GameManager.currRun.baseData.PlayerMoney;

        foreach (var item in GameManager.currRun.player.SideCards)
        {
            item.InitForChoice(true, SelectOwnedCard, 0);
            item.visuals.SetDraggable(true);
            (item.visuals as SideCardVisualHandler).OnStartDragEvent += visuals.LockSide;
            (item.visuals as SideCardVisualHandler).OnFinishDragEvent += visuals.UnlockSide;
        }


        GameManager.SaveGame();

    }

    public void InitScreen(SaveFormat save)
    {
        if (save.LockedHandCards == null)
        {
            LockedHandCards = new List<HandCardDataHandler>();
        }
        else
        {
            LockedHandCards = save.LockedHandCards;
        }

        if (save.LockedSideCards == null)
        {
            LockedSideCards = new List<SideCardDataHandler>();
        }
        else
        {
            LockedSideCards = save.LockedSideCards;
        }

        handCards = save.HandCards;
        sideCards = save.SideCards;

        visuals.init(handCards.ToArray(), sideCards.ToArray(), SelectCard);

        foreach (BaseCardDataHandler item in LockedHandCards)
        {
            try
            {
                ICardVisuals vis = handCards.Where(o => o.ID == item.ID).First().visuals;

                item.visuals = vis;

                visuals.ChangeLockState(vis, item.baseData.SideCard, true);
            }
            catch
            {
                Debug.LogError("Tried to find a Locked Hand Card that doesn't exist! Card name: " + item.baseData.Name + ". Please report this and the seed:" + GameManager.currRun.RunSeed);
            }
        }

        foreach (BaseCardDataHandler item in LockedSideCards)
        {
            try
            {
                ICardVisuals vis = sideCards.Where(o => o.ID == item.ID).First().visuals;

                item.visuals = vis;

                visuals.ChangeLockState(vis, item.baseData.SideCard, true);
            }
            catch
            {
                Debug.LogError("Tried to find a Locked Side Card that doesn't exist! Card name: " + item.baseData.Name + ". Please report this and the seed:" + GameManager.currRun.RunSeed);
            }
        }

        GameManager.currRun.runState = GameState.InShop;

        CanAct = true;

        RefreshCost = save.RefreshCost;
        RemoveCost = save.RemoveCost;
        SideCardCost = save.SideCardCost;

        visuals.UpdateRefreshPrice(RefreshCost);
        visuals.UpdateRemovePrice(RemoveCost);
        visuals.UpdateSidePrice(SideCardCost);

        currMoney = GameManager.currRun.baseData.PlayerMoney;

        foreach (var item in GameManager.currRun.player.SideCards)
        {
            item.InitForChoice(true, SelectOwnedCard, 0);
            item.visuals.SetDraggable(true);
            (item.visuals as SideCardVisualHandler).OnStartDragEvent += visuals.LockSide;
            (item.visuals as SideCardVisualHandler).OnFinishDragEvent += visuals.UnlockSide;
        }
    }

    public IEnumerator CloseShop()
    {
        foreach (var item in GameManager.currRun.player.SideCards)
        {
            (item.visuals as SideCardVisualHandler).OnStartDragEvent -= visuals.LockSide;
            (item.visuals as SideCardVisualHandler).OnFinishDragEvent -= visuals.UnlockSide;
        }

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnShopExit, GameManager.currRun.player, GameManager.currRun.shopData);

        GameManager.instance.StartCoroutine(GameManager.currRun.TriggerOnExitShop());
    }

    public HandCardDataHandler[] GenerateHandCards()
    {
        int amount = GameManager.STARTING_HAND_SHOP_AMOUNT;

        HandCardDataHandler[] cards = new HandCardDataHandler[amount];

        for (int i = 0; i < LockedHandCards.Count; i++)
        {
            cards[i] = LockedHandCards[i];
        }

        for (int i = LockedHandCards.Count; i < cards.Length; i++)
        {
            CardRarity rarity = GameManager.getRarity(GameManager.currRun.rarityData);

            HandCardSO[] rolledCards = GameManager.instance.AssetLibrary.FetchHandCards(o => o.Rarity == rarity);

            HandCardDataHandler newCard = new HandCardDataHandler(rolledCards[GameManager.currRun.ShopRand.NextInt(0, rolledCards.Length)], GameManager.currRun.player, true);

            cards[i] = newCard;
        }

        return cards;
    }

    public SideCardDataHandler[] GenerateSideCards()
    {
        int amount = GameManager.STARTING_SIDE_SHOP_AMOUNT;

        SideCardDataHandler[] cards = new SideCardDataHandler[amount];

        for (int i = 0; i < LockedSideCards.Count; i++)
        {
            cards[i] = LockedSideCards[i];
        }

        for (int i = LockedSideCards.Count; i < cards.Length; i++)
        {
            CardRarity rarity = GameManager.getRarity(GameManager.currRun.rarityData);

            SideCardSO[] rolledCards = GameManager.instance.AssetLibrary.FetchSideCards(o => o.Rarity == rarity);

            SideCardDataHandler newCard = new SideCardDataHandler(rolledCards[GameManager.currRun.ShopRand.NextInt(0, rolledCards.Length)], GameManager.currRun.player, true);

            cards[i] = newCard;
        }

        return cards;
    }

    public IEnumerator ActOnCard()
    {
        if (selectedCard != null)
        {
            if (selectedCard.temp)
            {

                bool canBuy = selectedCard is HandCardDataHandler || GameManager.currRun.player.CanAddSideCard();

                if (selectedCard != null && canBuy)
                {
                    Saudio.ShopActionSound();

                    int diff = selectedCard.price;
                    currMoney -= selectedCard.price;

                    if (selectedCard is SideCardDataHandler)
                    {
                        sideCards.Remove(selectedCard as SideCardDataHandler);

                        yield return visuals.BuySideCard(selectedCard.visuals);

                        selectedCard.changeChoice(SelectOwnedCard);

                        GameManager.currRun.player.AddSideCard(selectedCard as SideCardDataHandler);

                        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnBuy, this, selectedCard);

                        selectedCard.visuals.SetDraggable(true);
                        (selectedCard.visuals as SideCardVisualHandler).OnStartDragEvent += visuals.LockSide;
                        (selectedCard.visuals as SideCardVisualHandler).OnFinishDragEvent += visuals.UnlockSide;

                    }
                    else if (selectedCard is HandCardDataHandler)
                    {
                        handCards.Remove(selectedCard as HandCardDataHandler);

                        yield return visuals.BuyHandCard(selectedCard.visuals);

                        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnBuy, this, selectedCard);

                        yield return visuals.FinishBuyHandCard(selectedCard.visuals);

                        selectedCard.changeChoice(null);

                        GameManager.currRun.PlayerDeck.AddCardToBottomAtSpot(0, selectedCard as HandCardDataHandler);


                    }

                    selectedCard.temp = false;

                    GlobalSaveFormat.UnlockCard(selectedCard.baseData.Name, selectedCard.baseData.SideCard);

                    TimesBought++;

                    yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, -diff);
                }

                selectedCard = null;
            }
            else
            {
                Saudio.ShopActionSound();

                GameManager.currRun.player.RemoveSideCard(selectedCard as SideCardDataHandler);
                int diff = Mathf.CeilToInt(selectedCard.price / 2f);
                currMoney += diff;

                yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, diff);
                yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnSellSide, this, selectedCard);

                //visuals.HideSellPrice();
                visuals.UnlockSide();

                selectedCard = null;
            }

        }
        GameManager.SaveGame();

        CanAct = true;
    }

    public void LockCard()
    {
        if (CanAct && selectedCard != null)
        {
            if (selectedCard.temp)
            {
                if (!LockedHandCards.Contains(selectedCard) && !LockedSideCards.Contains(selectedCard))
                {
                    if (selectedCard.baseData.SideCard)
                    {
                        LockedSideCards.Add(selectedCard as SideCardDataHandler);
                    }
                    else
                    {
                        LockedHandCards.Add(selectedCard as HandCardDataHandler);
                    }
                    visuals.ChangeLockState(selectedCard.visuals, selectedCard.baseData.SideCard, true);
                }

                else
                {
                    if (selectedCard.baseData.SideCard)
                    {
                        LockedSideCards.Remove(selectedCard as SideCardDataHandler);
                    }
                    else
                    {
                        LockedHandCards.Remove(selectedCard as HandCardDataHandler);
                    }
                    visuals.ChangeLockState(selectedCard.visuals, selectedCard.baseData.SideCard, false);
                }
            }  
        }
        GameManager.SaveGame();
    }

    public IEnumerator RemoveCard()
    {
        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        list = GameManager.currRun.player.currDeck.DeckBase.Select(o => new HandCardDataHandler(o) { TempData1 = o.ID }).ToList();

        deckViewForRemove.initMenu(list.OrderBy(o => o.returnUnmodifiedData().cardColors[0]).ToArray(), "Choose Card to remove", true, CardMenuChoiceMode.Open, MenuViewMode.Choice, 4);

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(this, deckViewForRemove.MakeChoice(1));

        yield return cd.coroutine;


        HandCardDataHandler card = null;

        if (cd.result[0] != null)
        {
            card = GameManager.currRun.player.currDeck.DeckBase.Where(o => o.ID == (long)cd.result[0].TempData1).FirstOrDefault();
        }

        if (card != null)
        {
            GameManager.currRun.PlayerDeck.RemoveCard(card);

            TimesRemoved++;

            if (!FreeRemove)
            {
                int diff = RemoveCost;
                RemoveCost += 2;
                currMoney -= (RemoveCost - 2);
                visuals.UpdateRemovePrice(RemoveCost);

                yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, -diff);
            }
            else
            {
                FreeRemove = false;
            }


            deckViewForRemove.CloseMenu();

            yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnRemove, this, card);
        }

        GameManager.SaveGame();
        CanAct = true;
    }

    public void RemoveButton()
    {
        if (CanAct)
        {
            CanAct = false;
            StartCoroutine(RemoveCard());
        }
    }

    public IEnumerator RefreshShop()
    {


        handCards = GenerateHandCards().ToList();
        sideCards = GenerateSideCards().ToList();

        visuals.ResetShop();

        foreach (var item in LockedHandCards)
        {
            item.Deload();
        }

        foreach (var item in LockedSideCards)
        {
            item.Deload();
        }

        yield return new WaitForGameEndOfFrame();

        //handCards.AddRange(LockedHandCards);
        //sideCards.AddRange(LockedSideCards);

        visuals.init(handCards.ToArray(), sideCards.ToArray(), SelectCard);

        foreach (BaseCardDataHandler item in LockedHandCards)
        {
            visuals.ChangeLockState(item.visuals, item.baseData.SideCard, true);
        }

        foreach (BaseCardDataHandler item in LockedSideCards)
        {
            visuals.ChangeLockState(item.visuals, item.baseData.SideCard, true);
        }

        TimesRefreshed++;

        if (!FreeRefresh)
        {
            int diff = RefreshCost;
            RefreshCost++;
            currMoney -= (RefreshCost - 1);

            visuals.UpdateRefreshPrice(RefreshCost);

            yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, -diff);


        }
        else
        {
            FreeRefresh = false;
        }

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnShopRefresh, this, this);
        GameManager.SaveGame();

        CanAct = true;
    }

    public IEnumerator BuySideCardSpace()
    {
        GameManager.currRun.IncreaseSideCardSize();

        int diff = SideCardCost;
        currMoney -= SideCardCost;
        visuals.UpdateSidePrice(SideCardCost);

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, -diff);
        GameManager.SaveGame();

        CanAct = true;
    }

    public void RefreshButton()
    {
        if (CanAct)
        {
            CanAct = false;
            if (selectedCard!= null)
            {
                visuals.DeselectCard(selectedCard.visuals, selectedCard.baseData.SideCard);
            }
            StartCoroutine(RefreshShop());
        }
    }

    public void SelectCard(BaseCardDataHandler card)
    {
        if (CanAct)
        {
            if (card == selectedCard)
            {
                visuals.DeselectCard(selectedCard.visuals, selectedCard.baseData.SideCard);
                selectedCard = null;
            }
            else
            {
                if (selectedCard != null)
                {
                    if (selectedCard.temp)
                    {
                        visuals.DeselectCard(selectedCard.visuals, selectedCard.baseData.SideCard);
                    }
                    else
                    {
                        selectedCard.visuals.SetSelect(false);
                        visuals.HideSellPrice();
                        visuals.UnlockSide();
                    }
                }

                selectedCard = card; 
                visuals.SelectCard(card.visuals, card.price, card.baseData.SideCard);


                visuals.UpdateBuyButton(currMoney >= card.price, false);
            }
        }
    }

    public void SelectOwnedCard(BaseCardDataHandler card)
    {
        if (CanAct)
        {
            if (card == selectedCard)
            {
                selectedCard.visuals.SetSelect(false);
                visuals.HideSellPrice();

                selectedCard = null;
            }
            else
            {
                if (selectedCard != null)
                {
                    if (selectedCard.temp)
                    {
                        visuals.DeselectCard(selectedCard.visuals, selectedCard.baseData.SideCard);
                    }
                    else
                    {
                        selectedCard.visuals.SetSelect(false);
                        visuals.HideSellPrice();
                        visuals.UnlockSide();
                    }
                }

                selectedCard = card;

                visuals.LockSide();
                visuals.ShowSellPrice(card.visuals, Mathf.CeilToInt(card.price / 2f));
                visuals.UpdateBuyButton(true, true);
            }
        }

        if (selectedCard == null)
        {
            visuals.UnlockSide();
        }
    }

    public void MakeFirstCardFree()
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i].price > 0)
            {
                handCards[i].price = 0;
                break;
            }
        }
    }

    public HandCardDataHandler MakeRandomCardFree()
    {
        HandCardDataHandler[] priced = handCards.Where(o => o.price > 0).ToArray();

        if (priced.Length > 0)
        {
            HandCardDataHandler card = priced[GameManager.currRun.ShopRand.NextInt(0, priced.Length)];
            card.price = 0;
            return card;
        }

        return null;
    }


    public bool CanMakeCardFree()
    {
        return handCards.Where(o => o.price > 0).Count() > 0;
    }

    public void BuyButton()
    {
        if (CanAct)
        {
            CanAct = false;
            StartCoroutine(ActOnCard());
        }
    }

    public void AddSideCardButton()
    {
        if (CanAct)
        {
            CanAct = false;
            StartCoroutine(BuySideCardSpace());
        }
    }

    public void ExitButton()
    {
        if (CanAct)
        {
            visuals.ResetShop();
            visuals.Deload();
            StartCoroutine(CloseShop());
        }
    }

    public void Deload()
    {
        try
        {
            visuals.Deload();
            if (handCards != null)
                handCards.Clear();
            if (LockedHandCards != null)
                LockedHandCards.Clear();
            if (sideCards != null)
                sideCards.Clear();
            if (LockedSideCards != null)
                LockedSideCards.Clear();
        }
        catch { }
    }

    public void SetScreenActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public IEnumerator MoveScreen(bool show)
    {
        yield return MoverHelper.MoveScreen(show);
    }

    public void PutScreen(bool show)
    {
        MoverHelper.PutScreen(show);
    }
}
