using Assets.Scripts.Auxilary;
using Assets.Scripts.Players.PlayerAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//The manager for a specific run. Will handle all the carriable resources the player has, as well as their deck outside of games.
public class RunManager 
{
    public bool CustomSeed;

    public string RunSeed;

    public int HandCardsShopAmount, SideCardsShopAmount;

    public DeckHandler PlayerDeck;

    public PlayerHandler player;
    public PlayerVisualManager playerVis;

    public PlayerData baseData;

    public ShopData shopData;

    public RarityChanceData rarityData;

    public GameState runState;

    public int ReplacePerRound, RemovePerRound, AddPerRound;

    public RoundLogicManager roundScene;
    public ShopLogicHandler shopScene;
    public EnemyChoiceDataHandler enemyChoiceScene;

    public bool MauPunishE, MauPunishP;
    public int MauCardE, MauCardP;

    public CustomRandom RoundRand, ShopRand;

    public RunManager(RoundLogicManager round, ShopLogicHandler shop, EnemyChoiceDataHandler enemySelect)
    {
        roundScene = round;
        shopScene = shop;
        enemyChoiceScene = enemySelect;

    }

    public void StartRun(DeckSO deck, bool customSeed, string newSeed)
    {
        CustomSeed = customSeed;
        RunSeed = newSeed;

        GenerateRandoms(newSeed, out RoundRand, out ShopRand);

        baseData = GameManager.instance.AssetLibrary.startRun.playerData;
        shopData = GameManager.instance.AssetLibrary.startRun.shopData;
        rarityData = GameManager.instance.AssetLibrary.startRun.rarityChanceData;

        playerVis = GameManager.instance.playerVis;

        player = new PlayerHandler(deck, playerVis);


        playerVis.Init();

        PlayerDeck = player.currDeck;

        baseData.PlayerMoney = 10;
    }

    public void LoadRun(SaveFormat save)
    {
        RunSeed = save.RunSeed;
        CustomSeed = save.IsCustomSeed;

        RoundRand = new CustomRandom(save.RoundSeed, save.RoundCount);
        ShopRand = new CustomRandom(save.ShopSeed, save.ShopCount);

        //GenerateRandoms(save.RunSeed, out RoundRand, out ShopRand);

        baseData = save.PlayerData;
        shopData = save.ShopData;
        rarityData = save.RarityChanceData;

        playerVis = GameManager.instance.playerVis;

        AudioManager.ShiftToRound();

        player = save.player;

        playerVis.Init();

        PlayerDeck = player.currDeck;
    }

    public void EndRun()
    {
        roundScene.StopAllCoroutines();
        shopScene.StopAllCoroutines();
        enemyChoiceScene.StopAllCoroutines();
        GameManager.instance.StopAllCoroutines();

        GlobalSaveFormat.EndRun(CalculateLikelyStrat(), CalculateLikelyColor());

        player.Deload();
        if (GameManager.instance.state == GameState.InRound)
        {
            roundScene.GetOpponent(player).Deload();
        }

        roundScene.MoverHelper.PutScreen(false);
        shopScene.MoverHelper.PutScreen(false);
        enemyChoiceScene.MoverHelper.PutScreen(false);

        roundScene.Deload();
        shopScene.Deload();
        enemyChoiceScene.Deload();

        PoolingManager.ReturnAllToPool();
        AudioManager.ShiftToMenu();
    }

    public IEnumerator StartShop()
    {
        shopScene.transform.parent.gameObject.SetActive(true);
        shopScene.InitScreen();

        yield return player.SetupSide();
        yield return shopScene.visuals.mover.MoveScreen(true);

        AudioManager.ShiftToShop();

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnShopEnter, GameManager.currRun, this);
        player.CanAct = true;
    }

    public void LoadShop(SaveFormat save)
    {
        player.InitOutOfRound(player, playerVis);
        shopScene.transform.parent.gameObject.SetActive(true);
        shopScene.InitScreen(save);


        GameManager.instance.StartCoroutine(player.SetupSide());
        shopScene.visuals.mover.PutScreen(true);

        AudioManager.ShiftToShop();

        playerVis.PutLayout(false);
        player.CanAct = true;

    }

    public IEnumerator StartChoice()
    {
        enemyChoiceScene.transform.parent.gameObject.SetActive(true);
        //baseData.RoundNum = 1;
        //Debug.Log("Remember to remove RoundNum = 1 for Side Card testing!");

        enemyChoiceScene.InitScreen(generateOpponent(), generateOpponent(), generateOpponent());

        AudioManager.ShiftToMenu();

        yield return new WaitForGameEndOfFrame();
        yield return enemyChoiceScene.visuals.mover.MoveScreen(true);
    }

    public void LoadChoice(SaveFormat save)
    {
        player.InitOutOfRound(player, playerVis);
        enemyChoiceScene.transform.parent.gameObject.SetActive(true);
        //baseData.RoundNum = 1;
        //Debug.Log("Remember to remove RoundNum = 1 for Side Card testing!");
        AudioManager.ShiftToMenu();

        enemyChoiceScene.InitScreen(save);
        playerVis.PutLayout(false);
        enemyChoiceScene.visuals.mover.PutScreen(true);
        player.CanAct = true;
    }

    public IEnumerator StartRound(NPCHandler opponent)
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        roundScene.transform.parent.gameObject.SetActive(true);
        roundScene.InitScreen(150, player, opponent);
        baseData.RoundNum++;
        GameManager.instance.StartCoroutine(list.CountCoroutine(roundScene.visuals.mover.MoveScreen(true)));
        GameManager.instance.StartCoroutine(list.CountCoroutine(playerVis.MoveLayout(true)));

        yield return list;

        AudioManager.ShiftToRound();

        roundScene.StartCoroutine(roundScene.HandleRound(false));

        Debug.Log(RunSeed);
    }

    public void LoadRound(SaveFormat save)
    {
        roundScene.transform.parent.gameObject.SetActive(true);

        roundScene.InitScreen(save);

        roundScene.visuals.mover.PutScreen(true);
        playerVis.PutLayout(true);

        //baseData.RoundNum = save.RoundNum;
        roundScene.AnteAmount = save.RoundAnte;

        roundScene.StartCoroutine(roundScene.HandleRound(true));

        Debug.Log(RunSeed);

    }

    public NPCHandler generateOpponent(/* Gonna require the type of AI to put into it and the type of deck */)
    {
        AIType type = (AIType)baseData.RoundNum;

        AIEvalTypes EvalType = (AIEvalTypes)RoundRand.NextInt(0, Enum.GetNames(typeof(AIEvalTypes)).Length);

        IAIBase AIBase = new SmartAI();
        Colors FavoriteColor = Colors.None;
        AIStrategy Strat = AIStrategy.None;

        if (type != AIType.Random)
        {
            Strat = GameManager.instance.EnemyAssetLibrary.ReturnValidStrat();
        }

        switch (type)
        {
            case AIType.Random:
                AIBase = new RandomAI();
                break;
            case AIType.Dumb:
                AIBase = new DumbAI();
                break;
            case AIType.Average:
                AIBase = new AverageAI();
                break;
            case AIType.Smart:
            case AIType.Master:
                AIStrategy Strat2 = AIStrategy.None;
                while (Strat2 == Strat)
                {
                    Strat2 = GameManager.instance.EnemyAssetLibrary.ReturnValidStrat();
                }
                AIBase = new MasterAI();               
                break;
            default:
                break;
        }

        AIBase.preferredColor = FavoriteColor;
        AIBase.preferredStrategy = Strat;
        AIBase.AIType = type;

        EnemyGenerationPoolSO currPool = GameManager.instance.EnemyAssetLibrary.GetEnemyPool(Strat);
        currPool.mainStrat = AIStrategy.All;

        AIBase.AIEvaluationValues = GameManager.instance.EnemyAssetLibrary.getValues(EvalType);

        List<HandCardSO> HandCards = currPool.GenerateDeck(baseData.RoundNum, FavoriteColor);
        List<SideCardSO> SideCards = currPool.GenerateSideCards(baseData.RoundNum);

        //SideCards.Add(GameManager.instance.AssetLibrary.FetchSideCard("Hue Picker"));

        return new NPCHandler(AIBase, HandCards, SideCards, currPool.startingMoney * baseData.RoundNum);
    }


    public IEnumerator TriggerOnEndRound()
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        baseData.PlayerMoney = player.RoundMoney;

        player.RoundMoney = 0;

        player.CanAct = false;

        baseData.PlayerMoney += GetRewardAmount();

        //roundScene.transform.parent.position = GameManager.instance.OutOfSightLoc.position;
        //
        //shopScene.transform.parent.position = Vector3.zero;

        GameManager.instance.StartCoroutine(list.CountCoroutine(playerVis.MoveLayout(false)));
        GameManager.instance.StartCoroutine(list.CountCoroutine(roundScene.visuals.mover.MoveScreen(false)));
        roundScene.transform.parent.gameObject.SetActive(false);

        yield return list;

        yield return GameManager.instance.StartCoroutine(StartShop());

    }

    public IEnumerator TriggerOnExitShop()
    {
        yield return shopScene.visuals.mover.MoveScreen(false);
        shopScene.transform.parent.gameObject.SetActive(false);


        //player.CanAct = false;

        //shopScene.transform.parent.position = GameManager.instance.OutOfSightLoc.position;
        //
        //enemyChoiceScene.transform.parent.position = Vector3.zero;

        yield return GameManager.instance.StartCoroutine(StartChoice());

    }

    public IEnumerator TriggerOnExitChoice(NPCHandler npc)
    {
        yield return enemyChoiceScene.visuals.mover.MoveScreen(false);
        enemyChoiceScene.transform.parent.gameObject.SetActive(false);


        //enemyChoiceScene.transform.parent.position = GameManager.instance.OutOfSightLoc.position;
        //
        //roundScene.transform.parent.position = Vector3.zero;
        
        yield return GameManager.instance.StartCoroutine(StartRound(npc));
    }

    public void IncreaseSideCardSize()
    {
        baseData.MaxSideCards++;
    }

    public int GetRewardAmount()
    {
        int amount = 10 * baseData.RoundNum;

        return amount;    
    }

    public int GetIntFromString(string s)
    {
        int seed = s[0] | (s[1] << 8) | (s[2] << 16) | (s[3] << 24);

        return seed;
    }

    public void GenerateRandoms(string initSeed, out CustomRandom rand1, out CustomRandom rand2)
    {
        string S1 = "";
        string S2 = "";

        int first = 0;
        int second = 0;

        for (int i = 0; i < initSeed.Length; i++)
        {
            if ((initSeed[i] & 1) != 0 && first < 4)
            {
                S1 += initSeed[i];
                first++;
            }
            else if(second < 4)
            {
                S2 += initSeed[i];
                second++;
            }
            else
            {
                S1 += initSeed[i];
                first++;
            }
        }

        rand1 = new CustomRandom(GetIntFromString(S1));
        rand2 = new CustomRandom(GetIntFromString(S2));
    }

    public Colors CalculateLikelyColor()
    {
        List<Colors> mostUsedColors = new List<Colors>();

        var temp = player.currDeck.DeckBase.GroupBy(o => o.ReturnMainColor()).Select(group => new { color = group.Key, count = group.Count() }).ToList();

        int max = temp.Max(o => o.count);

        foreach (var item in temp)
        {
            if (item.count == max)
            {
                mostUsedColors.Add(item.color);
            }
        }

        Debug.Log(temp);

        if (mostUsedColors.Count == 0 || mostUsedColors.Count > 2)
        {
            return Colors.None;
        }
        else
        {
            return mostUsedColors[0];
        }
    }

    public AIStrategy CalculateLikelyStrat()
    {
        List<AIStrategy> mostUsedStrats = new List<AIStrategy>();

        Dictionary<AIStrategy, int> counters = new Dictionary<AIStrategy, int>();

        foreach (var item in player.currDeck.DeckBase)
        {
            
            foreach (var strat in item.data.AIStrategyType)
            {
                if (counters.ContainsKey(strat))
                {
                    counters[strat]++;
                }
                else
                {
                    counters.Add(strat, 1);
                }
            }
        }

        foreach (var item in player.SideCards)
        {
            foreach (var strat in item.data.AIStrategyType)
            {
                if (counters.ContainsKey(strat))
                {
                    counters[strat]++;
                }
                else
                {
                    counters.Add(strat, 1);
                }
            }
        }

        int max = counters.Max(o => o.Value);

        foreach (var item in counters)
        {
            if (item.Value == max)
            {
                mostUsedStrats.Add(item.Key);
            }
        }

        if (mostUsedStrats.Count == 0 || mostUsedStrats.Count > 2)
        {
            return AIStrategy.None;
        }
        else
        {
            return mostUsedStrats[0];
        }
    }
}
