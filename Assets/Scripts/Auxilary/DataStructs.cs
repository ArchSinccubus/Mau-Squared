using System;
using System.Collections.Generic;
using UnityEngine;


#region Enums
public enum MenuViewMode { View, Choice }

public enum CollectionType { Hand, Side, Invitation, Deck }

public enum CardRarity { Common, Uncommon, Rare, Legendary, Mythic }

public enum CardValue { Numeric, Wild, None }

public enum Colors { Red, Orange, Blue, Green, None }

public enum MovementOption { Space, Time }

public enum HandCardState { InDeck, InHand, InPlay, InPile, InMenu }

public enum DictionaryTypes { OnTurnStart, OnTurnEnd, OnPicked, OnRecycle, OnCardPlayed, OnCardPlaced, OnCardSmoked, OnCardCleared, OnSmokedPlayed, OnPreRoundStart, OnRoundStart,
    OnRoundEnd, OnDraw, OnScoreChanged, OnRemove, OnBuy, OnSellSide, OnShopRefresh, OnShopEnter, OnShopExit, OnMoneyChanged, OnActivateSideCard }

public enum GameState { InMenu, InRound, InShop, inChoice, InResults }

public enum RoundState { StartRound, StartTurn, Turn, DrawForTurn, EndTurn, EndRound }

public enum GameSpeed { Slow, Regular, Fast, VeryFast }

public enum CardMenuChoiceMode { Forced, Semi, Open, Null }

public enum EventShiftOptions { First, Up, Down, End }

#region AIEnums
public enum AIType { Random, Dumb, Average, Smart, Master }

public enum HandChoiceType {None, Smoke, Clean, Recycle, Upgrade, Transform, PlayExtra,  Other }

public enum SetChoiceType { None, Transform, HandAdd, Effect, PreColor, PreValue, Other }

public enum AIStrategy {None, HandEmpty, HighScore, SmokeOpponent, SmokeSelf, GetMoney, PileControl, Tarot, PieceGather, All, Random}

public enum AIEvalTypes { Average, Careful, LovesColors, LovesValues, Carefree, AnythingGoes }
#endregion

#endregion

#region Structs
public struct EventDataArgs
{
    public object Caller;
    public object Sender;
    public object Data;
}

[System.Serializable]
public struct PlayerData
{
    public int StartHandSize;
    public int MaxSideCards;

    public int PlayerMoney;

    public int CardsPlayed;

    public int OutOfOptionsDrawAmount;

    public int CardsRecycled;

    public int TotalScore;

    public int RoundNum;


}

[System.Serializable]
public struct RarityChanceData
{
    public float CommonWeight;
    public float UncommonWeight;
    public float RareWeight;
    public float MythicWeight;
}

[System.Serializable]
public struct ShopData
{
    public int TimesBoughtHand;
    public int TimesBoughtSide;
    public int TimesRefreshedShop;
    public int TimesRemoved;
    public int TimesSoldSide;

    public int RefreshCostStart;
    public int RemoveCostStart;
    public int AddSideCardSpaceCostStart;
}

[System.Serializable]
public struct HandCardData : IEquatable<HandCardData>
{
    public string Name;

    [SerializeField]
    public List<int> cardValues;
    [SerializeField]
    public List<Colors> cardColors;
    [SerializeField]
    public int Score;
    [SerializeField]
    public float Mult;
    [SerializeField]
    public bool PreWild;
    [SerializeField]
    public bool PostWild;

    public bool Equals(HandCardData other)
    {
        return this.cardValues == other.cardValues && 
            this.cardColors == other.cardColors && 
            this.Score == other.Score && 
            this.Mult == other.Mult && 
            this.PreWild == other.PreWild && 
            this.PostWild == other.PostWild;
    }
}

public struct DiscardEventArgs
{
    public HandCardDataHandler[] cards;
    public EntityHandler owner;
}

public struct EventDataStruct
{ 
    public ISubscriber Value;
    public object Key;
}
 
public struct EndRoundData
{
    public EntityHandler winner;
    public HandCardDataHandler endCard;
}

public struct SmokeEventData
{ 
    public List<HandCardDataHandler> cards;
    public EntityHandler cause;
}

public struct CardDecorateData
{
    public ICardDecorator decorator;
    public object Caller;
}

public struct PlayerDecorateData
{
    public object Caller;
    public PlayerData data;
}

#region Enemy Data Structs
[System.Serializable]
public struct EnemyHandCardData
{
    public HandCardSO Card;
    public int limit;
    public float Weight;
}

[System.Serializable]
public struct EnemySideCardData
{
    public SideCardSO Card;
    public float Weight;
}

[System.Serializable]
public struct AIEvaluationValues
{
    [Range(0,1)]
    public float ColorValue;
    [Range(0, 1)]
    public float NoColorValue;
    [Range(0, 1)]
    public float RarityMult;
    [Range(0, 1)] 
    public float CardValueValue;
    [Range(1, 5)] 
    public float StrategyMult;
    [Range(0, 1)] 
    public float NoStrategyMult;
    [Range(0, 1)] 
    public float CutoffValue;
    [Range(0, 1)] 
    public float SideCardChanceStrat;
    [Range(0, 1)] 
    public float SideCardChanceNoStrat;
}

public class AICardScorePair
{
    public HandCardDataHandler Card;
    public float Score;
}

#endregion

#region SaveFormats

[Serializable]
public struct CardPileStruct
{
    public HandCardDataHandler card;
    public bool OwnerPlayer;
    public Vector2 position;
    public Quaternion rotation;
}

#endregion

#endregion