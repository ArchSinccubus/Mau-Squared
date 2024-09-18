using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Assets.Scripts.Auxilary;
using Unity.VisualScripting;

public delegate IEnumerator PlayCardMethod(params HandCardDataHandler[] Cards);

public class RoundLogicManager : MonoBehaviour, IGameScreen
{
    public PlayCardMethod PlayMethod;

    public EntityHandler TurnPlayer;
    public List<EntityHandler> PlayersInRound;

    public List<HandCardDataHandler> Pile;

    public RoundVisualManager visuals;
    public RoundAudioManager sounds;

    public EntityHandler Winner;

    public CardMenuHandler CardViewer;

    public ClickableObject PileClick;

    public int AnteAmount;

    public int TurnCount;

    public RoundState state;

    public int MaxPlayerCardScore;
    public int TimesSmoked, TimesRecycled, TimesCleaned;

    Transform IGameScreen.transform { get => transform; }
    public ScreenMoverHelper MoverHelper { get => visuals.mover; }

    public void InitScreen(int ante, params EntityHandler[] players)
    {
        Pile = new List<HandCardDataHandler>();

        GameManager.instance.currRound = this;

        PlayersInRound = new List<EntityHandler>();

        foreach (var item in players)
        {
            PlayersInRound.Add(item);
            item.InitNewRound(visuals.players[item is PlayerHandler ? 0 : 1], 
                              sounds.players[item is PlayerHandler ? 0 : 1]);
        }

        GameManager.currRun.runState = GameState.InRound;

        AnteAmount = ante;

        Winner = null;

        TurnPlayer = GameManager.currRun.player;

        TurnCount = 0;

        PileClick.AddDelegate(ViewPile);

        visuals.Init(ante);

        MaxPlayerCardScore = 0;
    }

    public void InitScreen(SaveFormat save)
    {
        Pile = new List<HandCardDataHandler>();


        PlayersInRound = new List<EntityHandler>
        {
            save.player,
            save.NPC
        };

        foreach (var data in save.PileData)
        {
            data.card.owner = PlayersInRound[data.OwnerPlayer ? 0 : 1];

            Pile.Add(data.card);
            data.card.InitForPile();
            data.card.state = HandCardState.InPile;
            //data.card.visuals.SetPos(data.position);
            //data.card.visuals.transform.rotation = data.rotation;

            visuals.PutCardToPile(data.card.visuals, data.position, data.rotation);
        }

        save.player.InitNewRound(save.player, visuals.players[0], sounds.players[0]);
        save.NPC.InitNewRound(save.NPC, visuals.players[1], sounds.players[1]);

        save.player.visuals.ScoreText.SetTextInstant(save.player.GetScore());
        save.NPC.visuals.ScoreText.SetTextInstant(save.NPC.GetScore());

        GameManager.currRun.runState = GameState.InRound;

        TurnPlayer = save.player;

        AnteAmount = save.RoundAnte;

        Winner = null;
        TurnCount = save.TurnNum;

        PileClick.AddDelegate(ViewPile);

        visuals.Init(save.RoundAnte);

        MaxPlayerCardScore = save.MaxPlayerCardScore;

        TimesCleaned = save.Cleaned;
        TimesRecycled = save.Recycled;
        TimesSmoked = save.Smoked;
    }

    public IEnumerator StartRound()
    {
        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnPreRoundStart, this, TurnPlayer);

        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (EntityHandler player in PlayersInRound)
        {
            StartCoroutine(list.CountCoroutine(player.StartRound()));
        }

        yield return list;

        state = RoundState.StartRound;
        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnRoundStart, this, TurnPlayer);
    }

    public IEnumerator StartLoadRound()
    {
        CoroutineWaitForList list = new CoroutineWaitForList();


        foreach (EntityHandler player in PlayersInRound)
        {
            StartCoroutine(list.CountCoroutine(player.LoadRound()));
        }

        yield return list;
    }

    public EntityHandler GetOpponent(EntityHandler entity)
    {
        return PlayersInRound[Mathf.Abs(1 - PlayersInRound.IndexOf(entity))];
    }

    public NPCHandler getNPC()
    { 
        return PlayersInRound.Where(o => o is NPCHandler).ElementAt(0) as NPCHandler;
    }

    public EntityHandler GetTrueWinner()
    {
        PlayersInRound = PlayersInRound.OrderByDescending(o => o.GetScore()).ToList();

        return PlayersInRound[0];
    }

    public HandCardDataHandler ReturnTopCardData()
    {
        int ID = Pile.Count - 1;

        if (ID >= 0)
        {
            HandCardDataHandler card = Pile[ID];


            while (card.ignore && ID - 1 >= 0)
            {
                ID -= 1;
                card = Pile[ID];
            }

            if (ID >= 0)
            {
                return card;
            }
        }

        return null;
    }

    public IEnumerator EndRound()
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        state = RoundState.EndRound;

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnRoundEnd, this, new EndRoundData() { winner = TurnPlayer, endCard = ReturnTopCardData() });

        yield return Winner.AddScore(AnteAmount * Winner.RoundMoney);

        Winner = GetTrueWinner();

        foreach (var card in Pile) 
        {
            if (card.owner == GameManager.currRun.player && !card.temp)
            {
                card.visuals.transform.rotation = Quaternion.identity;
                StartCoroutine(list.CountCoroutine(GameManager.currRun.player.visuals.AddToDeck(card.visuals)));
                //StartCoroutine(list.CountCoroutine(card.visuals.Flip(false)));
                GameManager.currRun.player.currDeck.AddCardToShuffle(card);
                yield return new WaitForGameSeconds(0.1f);
            }
            else
            {
                card.ClearForRound();
            }
        }

        yield return list;

        if (Winner == GameManager.currRun.player)
        {
            sounds.PlayerWin();
            Debug.Log("Player won! Going to store now.");
            foreach (var item in PlayersInRound)
            {
                yield return item.EndRound();
            }

        }
        else
        {
            //Again, scuffed up shit. More than you can possibly imagine
            SideCardDataHandler[] deaths = GameManager.currRun.player.SideCards.Where(o => o.data is DeathSO).ToArray();
            if (deaths.Length > 0)
            {
                yield return (deaths[0].data as DeathSO).RemoveCard(deaths[0]);
                foreach (var item in PlayersInRound)
                {
                    yield return item.EndRound();
                }
            }
            else
            {
                sounds.PlayerLose();
                Debug.Log("Player lost! Booooooo :(");
            }
        }

        GlobalSaveFormat.EndRound(GameManager.currRun.player.currDeck.DeckBase, GameManager.currRun.player.SideCards, GameManager.currRun.player.RoundMoney, TimesSmoked, TimesCleaned, TimesRecycled, GameManager.currRun.player.GetScore(), MaxPlayerCardScore);

        PileClick.RemoveDelegate(ViewPile);
        yield return GameManager.instance.ResultMenu.InitResults(Winner == GameManager.currRun.player);

    }

    public IEnumerator StartTurn(bool load)
    {      
        TurnPlayer = PlayersInRound[0];
        PlayersInRound[0] = PlayersInRound[PlayersInRound.Count - 1];
        PlayersInRound[PlayersInRound.Count - 1] = TurnPlayer;

        yield return visuals.StartNewTurn(TurnCount);
        state = RoundState.StartTurn;
        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnTurnStart, this, TurnPlayer);
    }

    public IEnumerator EndTurn()
    {

        foreach (var item in PlayersInRound)
        {
            if (item.Hand.Count == 0)
            {
                Winner = item;
            }
        }

        state = RoundState.EndTurn;
        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnTurnEnd, this, TurnPlayer);

    }

    public void AddCardToPile(HandCardDataHandler card)
    {
        Pile.Add(card);
    }

    public IEnumerator HandleRound(bool load)
    {
        if (!load)
        {
            yield return StartRound();
        }
        else
        {
            yield return StartLoadRound();
        }

        while (Winner == null)
        {
            if (!load)
            {
                if (TurnPlayer == GameManager.currRun.player)
                {
                    TurnCount++;
                }
            }

            yield return visuals.ShowTurnChange(TurnCount);

            yield return StartTurn(load);

            load = false;

            state = RoundState.Turn;
            yield return TurnPlayer.ExecuteTurn();

            if (TurnPlayer.selectedCard != null)
            {
                yield return TurnPlayer.selectedCard.PlayCard();
            }
            else
            {
                state = RoundState.DrawForTurn;
                yield return TurnPlayer.Draw(TurnPlayer.playerData.OutOfOptionsDrawAmount);
            }

            yield return EndTurn();
        }

        yield return EndRound();
    }

    public IEnumerator PlayCard(params HandCardDataHandler[] cards)
    {
        yield return PlayMethod(cards);
    }

    public Vector2 GetPileLocation()
    {
        return visuals.PileLoc.position;
    }

    public void CompareScores(int score)
    {
        if (score > MaxPlayerCardScore)
        {
            MaxPlayerCardScore = score;
        }
    }

    public IEnumerator MakeChoice(HandCardDataHandler[] options, string text, int amount, CardMenuChoiceMode mode)
    { 
        CardViewer.initMenu(options, text, false, mode, MenuViewMode.Choice, 1);

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(this, CardViewer.MakeChoice(amount));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        CardViewer.CloseMenu();

        yield return cd.result;
    }

    public IEnumerator MakeHandChoice(EntityHandler entity, string text, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query)
    {
        CardViewer.initHandChoice(text, false, mode);

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(this, CardViewer.MakeHandChoice(entity.visuals.HandLoc,entity.Hand, amount, query));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        CardViewer.CloseHandMenu();

        yield return cd.result;
    }

    public void ViewPile()
    {
        if (GameManager.currRun.player.CanAct && Pile.Count > 0)
        {
            HandCardDataHandler[] array = Pile.ConvertAll(o => o = new HandCardDataHandler(o)).ToArray();

            GameManager.Round.CardViewer.initMenu(array, "", true, CardMenuChoiceMode.Null, MenuViewMode.View, 4);
            GameManager.Round.CardViewer.OpenMenu();
        }
    }

    public void Deload()
    {
        try {
            visuals.Deload();
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
