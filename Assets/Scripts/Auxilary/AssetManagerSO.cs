using System;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Assets.Scripts.Auxilary;

[CreateAssetMenu(fileName = "New Asset Manager", menuName = "Mau/New Asset Manager")]
public class AssetManagerSO : ScriptableObject
{
    public StartingRunData startRun;

    [SerializeField]
    TextAsset HandDescriptions;

    [SerializeField]
    TextAsset SideDescriptions;

    JObject HandDescs, SideDescs;

    [SerializeField]
    public UDictionary<string, CurveContainerScriptableObject> curves;

    [SerializeField]
    UDictionary<string, DeckSO> decks;

    [SerializeField]
    HandCardSO HandCardTemplate;

    [SerializeField]
    UDictionary<string, BasicCardSO> BasicCards;

    [SerializeField]
    public UDictionary<string, HandCardSO> HandCards;

    [SerializeField]
    public UDictionary<string, SideCardSO> SideCards;

    [SerializeField]
    UDictionary<AIType, EnemyGenerationPoolSO> EnemyPools;

    [SerializeField]
    UDictionary<CardRarity, Sprite> RarityBorders;

    public void Init()
    { 
        HandDescs = JObject.Parse(HandDescriptions.text);
        SideDescs = JObject.Parse(SideDescriptions.text);
    }

    public DeckSO FetchDeck(string name)
    {
        return decks[name];
    }

    public string GetHandDescription(string name)
    {
        if (HandDescs.ContainsKey(name))
        {
            try
            {
                var test = HandDescs[name];

                return HandDescs[name]["Text"].ToString();
            }
            catch
            {
                //if (!name.Contains("of"))
                //{
                //    Debug.LogError(name + "'s Text couldn't be found! Fix this lmao");
                //}
                return "";
            }
        }

        return "";
    }

    public string GetHandTrigger(string name)
    {
        try
        {
            return HandDescs[name]["Trigger"].ToString();
        }
        catch
        {
            Debug.LogError(name + "'s Trigger text couldn't be found! Fix this lmao");

            return "";
        }
    }

    public string GetHandPlayed(string name)
    {
        try
        {
            return HandDescs[name]["Played"].ToString();
        }
        catch
        {
            Debug.LogError(name + "'s Played text couldn't be found! Fix this lmao");
            return "";
        }
    }

    public string GetHandDrawn(string name)
    {
        try
        {
            return HandDescs[name]["Drawn"].ToString();
        }
        catch
        {
            Debug.LogError(name + "'s Drawn text couldn't be found! Fix this lmao");
            return "";
        }
    }

    public string GetHandChoiceText(string name)
    {
        try
        {
            return HandDescs[name]["ChoiceText"].ToString();
        }
        catch
        {
            Debug.Log("Tried to find choice text for " + name + " which should not happen");
            return "";
        }
    }

    public JToken GetAdventureTimeStuff(string name)
    {
        return HandDescs[name];
    }

    public HandCardSO FetchHandCard(string name) 
    {
        if (HandCards.ContainsKey(name))
        {
            return HandCards[name];
        }
        return BasicCards[name];
    }

    public HandCardSO FetchRandomHandCard(CustomRandom rand)
    {
        HandCardSO card = HandCards.ElementAt(rand.NextInt(0, HandCards.Count)).Value;

        while (card is MauSO)
        {
            card = HandCards.ElementAt(rand.NextInt(0, HandCards.Count)).Value;
        }

        return card;
    }

    public HandCardSO[] FetchRandomHandCards(CustomRandom rand, int amount)
    {
        HandCardSO[] cards = rand.GetRandomElements(HandCards.Values, amount).ToArray();

        while (cards.Where(o => o is MauSO).Count() > 0)
        {
            cards = rand.GetRandomElements(HandCards.Values, amount).ToArray();
        }

        return cards;
    }

    public HandCardSO FetchRandomHandCard(CustomRandom rand, Func<HandCardSO, bool> query)
    {
        HandCardSO card = rand.GetRandomElements(HandCards.Values.Where(query).ToList(), 1)[0];

        while (card is MauSO)
        {
            card = rand.GetRandomElements(HandCards.Values.Where(query).ToList(), 1)[0];
        }

        return card;
    }

    public HandCardSO[] FetchRandomHandCards(CustomRandom rand, Func<HandCardSO, bool> query, int amount)
    {
        HandCardSO[] cards = rand.GetRandomElements(HandCards.Values.Where(query).ToList(), amount).ToArray();

        while (cards.Where(o => o is MauSO).Count() > 0)
        {
            cards = rand.GetRandomElements(HandCards.Values.Where(query).ToList(), amount).ToArray();
        }

        return cards;
    }

    public HandCardSO[] FetchHandCards(Func<HandCardSO, bool> query)
    {
        return HandCards.Values.Where(query).ToArray();
    }

    public HandCardSO FetchHandCardTemplate()
    {
        return HandCardTemplate;
    }

    public SideCardSO FetchSideCard(string name)
    { 
        return SideCards[name];
    }

    public string GetSideDescription(string name)
    {
        return SideDescs[name]["Text"].ToString();
    }

    public string GetSideChoiceText(string name)
    {
        return SideDescs[name]["ChoiceText"].ToString();
    }

    public string GetSideTrigger(string name)
    {
        try
        {
            return SideDescs[name]["Trigger"].ToString();
        }
        catch
        {
            return "";
        }
    }

    public SideCardSO FetchRandomSideCard(CustomRandom rand)
    {
        return SideCards.ElementAt(rand.NextInt(0, HandCards.Count)).Value;
    }

    public SideCardSO FetchRandomSideCard(CustomRandom rand, Func<SideCardSO, bool> query)
    {
        return rand.GetRandomElements(SideCards.Values.Where(query).ToList(), 1)[0];
    }

    public SideCardSO[] FetchRandomSideCards(CustomRandom rand, Func<SideCardSO, bool> query, int amount)
    {
        return rand.GetRandomElements(SideCards.Values.Where(query).ToList(), amount).ToArray();
    }

    public SideCardSO[] FetchSideCards(Func<SideCardSO, bool> query)
    {
        return SideCards.Values.Where(query).ToArray();
    }

    public EnemyGenerationPoolSO returnPool(AIType type)
    {
        return EnemyPools[type];
    }

    public Sprite GetBorder(CardRarity rarity)
    {
        return RarityBorders[rarity];
    }

    #region Debug and utility methods;
    public void Clean()
    {
        BasicCards.Clear();
        HandCards.Clear();
        SideCards.Clear();
    }

    public void InsertBasicCard(BasicCardSO card)
    {
        string name = card.Name.Replace("'", "");
        BasicCards.Add(name, card);
    }

    public void InsertHandCard(HandCardSO card)
    {
        string name = card.Name.Replace("'", ""); 
        HandCards.Add(name, card);
    }

    public void InsertSideCard(SideCardSO card)
    {
        string name = card.Name.Replace("'", "");
        SideCards.Add(name, card);
    }

    public void InsertDeck(DeckSO deck)
    {
        decks.Add(deck.name, deck);
    } 
    #endregion
}
