using Assets.Scripts.Auxilary;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalSaveFormat
{
    /// <summary>
    /// Way stats are counted:
    /// -1 - Card was never locked
    /// 0 - Card was bought - Unlock it
    /// Side Card Usage counted each round finished with that card
    /// Hand Card Usage counted each round finished with that card in your deck (Even if not played!)
    /// </summary>

    [JsonIgnore]
    public static GlobalSaveFormat instance;

    [JsonProperty]
    public Dictionary<string, int> UnlockedHandCards;

    [JsonProperty]
    public Dictionary<string, int> UnlockedSideCards;

    [JsonProperty]
    public SaveFormat LastRun;

    [JsonProperty]
    internal Dictionary<AIStrategy, int> StratCount;
    public AIStrategy FaveStrat;

    [JsonProperty]
    internal Dictionary<Colors, int> ColorCount;
    public Colors FaveColor;

    public string FaveHandCard;

    public string FaveSideCard;

    public int TimesRecycled;

    public int TimesSmoked;

    public int TimesCleaned;

    public int HighestMoney;

    public int HighestSingleCardScore;

    public int HighestRoundScore;

    public GlobalSaveFormat(GlobalSaveFormatSO basis)
    {
        UnlockedHandCards = new Dictionary<string, int>();
        UnlockedSideCards = new Dictionary<string, int>();
        StratCount = new Dictionary<AIStrategy, int>();
        ColorCount = new Dictionary<Colors, int>();

        foreach (var item in basis.UnlockedSideCards)
        {
            UnlockedSideCards.Add(item.Key, item.Value);
        }

        foreach (var item in basis.UnlockedHandCards)
        {
            UnlockedHandCards.Add(item.Key, item.Value);
        }

        foreach (var item in basis.ColorCount)
        {
            ColorCount.Add(item.Key, 0);
        }

        foreach (var item in basis.StratCount)
        { 
            StratCount.Add(item.Key, 0);
        }

        FaveHandCard = "";
        FaveSideCard = "";

        HighestMoney = 0;

        FaveColor = Colors.None;
        FaveStrat = AIStrategy.None;

        instance = this;     
    }

    [JsonConstructor]
    public GlobalSaveFormat(Dictionary<string, int> unlockedHandCards, Dictionary<string, int> unlockedSideCards, SaveFormat lastRun, Dictionary<AIStrategy, int> stratCount, AIStrategy faveStrat, Dictionary<Colors, int> colorCount, Colors faveColor, string faveHandCard, string faveSideCard, int timesRecycled, int timesSmoked, int timesCleaned, int highestMoney, int highestSingleCardScore, int highestRoundScore)
    {
        UnlockedHandCards = unlockedHandCards;
        UnlockedSideCards = unlockedSideCards;
        LastRun = lastRun;
        StratCount = stratCount;
        FaveStrat = faveStrat;
        ColorCount = colorCount;
        FaveColor = faveColor;
        FaveHandCard = faveHandCard;
        FaveSideCard = faveSideCard;
        TimesRecycled = timesRecycled;
        TimesSmoked = timesSmoked;
        TimesCleaned = timesCleaned;
        HighestMoney = highestMoney;
        HighestSingleCardScore = highestSingleCardScore;
        HighestRoundScore = highestRoundScore;

        instance = this;
    }

    public static void UnlockCard(string name, bool Side)
    {
        string index = name.Replace("'", "");

        if (Side)
        {
            if (instance.UnlockedSideCards[index] == -1)
            {
                instance.UnlockedSideCards[index] = 0;
            }
        }
        else
        {
            if (instance.UnlockedHandCards[index] == -1)
            {
                instance.UnlockedHandCards[index] = 0;
            }
        }

        GameManager.SaveGlobal();
    }

    public static void CompareRoundScores(int newScore)
    {
        if (instance.HighestRoundScore < newScore)
        {
            instance.HighestRoundScore = newScore;
        }
    }

    public static void CompareHandScores(int newScore)
    {
        if (instance.HighestSingleCardScore < newScore)
        {
            instance.HighestSingleCardScore = newScore;
        }
    }

    public static void CalculateStratForRun(AIStrategy strat)
    {
        instance.StratCount[strat]++;

        int max = instance.StratCount.Max(o => o.Value);

        instance.FaveStrat = instance.StratCount.Where(o => o.Value == max).Select(o => o.Key).First();
    }

    public static void CalculateColorForRun(Colors color)
    {
        instance.ColorCount[color]++;

        int max = instance.ColorCount.Max(o => o.Value);

        instance.FaveColor = instance.ColorCount.Where(o => o.Value == max).Select(o => o.Key).First();
    }

    public static void EndRound(List<HandCardDataHandler> deck, List<SideCardDataHandler> sides, int RoundMoney, int SmokeAmount, int CleanAmount, int RecycleAmount, int RoundScore, int CardScore) 
    {
        foreach (var item in deck)
        {
            if (instance.UnlockedHandCards.ContainsKey(item.data.Name))
            {
                instance.UnlockedHandCards[item.data.name]++;
            }
        }

        foreach (var item in sides)
        {
            if (instance.UnlockedSideCards.ContainsKey(item.data.Name))
            {
                instance.UnlockedSideCards[item.data.name]++;
            }
        }

        if (instance.HighestMoney < RoundMoney)
        {
            instance.HighestMoney = RoundMoney;
        }

        instance.TimesSmoked += SmokeAmount;

        instance.TimesCleaned += SmokeAmount;

        instance.TimesRecycled += SmokeAmount;

        CompareRoundScores(RoundScore);

        CompareHandScores(CardScore);

        GameManager.SaveGlobal();
    }

    public static void EndRun(AIStrategy strat, Colors color)
    {
        CalculateColorForRun(color);

        CalculateStratForRun(strat);

        GameManager.SaveGlobal();
    }

    public static string GetFavoriteHandCard()
    { 
        string name = instance.UnlockedHandCards.Where(o => o.Value > 0).OrderByDescending(o => o.Value).FirstOrDefault().Key;

        if (name != null && name != "") 
        {
            instance.FaveHandCard = name;
            return name;
        }

        instance.FaveHandCard = "";
        return "";
    }

    public static string GetFavoriteSideCard()
    {
        string name = instance.UnlockedSideCards.Where(o => o.Value > 0).OrderByDescending(o => o.Value).FirstOrDefault().Key;

        if (name != null && name != "")
        {
            instance.FaveSideCard = name;
            return name;
        }

        instance.FaveSideCard = "";
        return "";
    }
}

