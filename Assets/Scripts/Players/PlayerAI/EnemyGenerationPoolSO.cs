using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Type" , menuName = "Mau/New Enemy Type")]
public class EnemyGenerationPoolSO : ScriptableObject
{
    public AIStrategy mainStrat;

    public int startingMoney;

    public List<EnemyHandCardData> possibleHandCards;

    public List<EnemySideCardData> possibleSideCards;

    public HandCardSO FetchWeightedHandCard(int counted)
    {
        float total = 0;
        int ID = possibleHandCards.Count - 1;


        foreach (var item in possibleHandCards)
        {
            if (item.limit > counted)
            {
                total += item.Weight;
            }
        }

        float rand = GameManager.currRun.RoundRand.NextFloat(0, total);

        while ((rand - possibleHandCards[ID].Weight) > 0)
        {
            rand -= possibleHandCards[ID].Weight;
            ID--;
        }

        return possibleHandCards[ID].Card;
    }

    public SideCardSO FetchWeightedSideCard()
    {
        float total = 0;
        int ID = possibleSideCards.Count - 1;


        foreach (var item in possibleSideCards)
        {
            total += item.Weight;
        }

        float rand = GameManager.currRun.RoundRand.NextFloat(0, total);

        while ((rand - possibleSideCards[ID].Weight) > 0)
        {
            rand -= possibleSideCards[ID].Weight;
            ID--;
        }

        return possibleSideCards[ID].Card;
    }

    public List<HandCardSO> GenerateDeck(int RoundNum, Colors favoriteColor)
    {
        //Remove later, just to disable
        //RoundNum = 1;

        List<HandCardSO> initialDeck = new List<HandCardSO>();

        foreach (var item in GameManager.instance.AssetLibrary.FetchDeck("Basic").DeckBase)
        {
            initialDeck.Add(item);
        }

        int replaceAmountFinal = RoundNum * (GameManager.BASE_CARD_REPLACEMENT_NUM + GameManager.getDifficultyModifier());

        int removeAmountFinal = RoundNum * GameManager.currRun.RoundRand.NextInt(0, (GameManager.BASE_CARD_REMOVAL_NUM + GameManager.getDifficultyModifier()) + 1);

        int addAmountFinal = RoundNum * GameManager.currRun.RoundRand.NextInt(0, (GameManager.BASE_CARD_ADDITION_NUM + GameManager.getDifficultyModifier()) + 1);

        List<HandCardSO> cardsToActOn = GameManager.currRun.RoundRand.GetRandomElements(initialDeck.Where(o => !o.cardColors.Contains(favoriteColor)).ToList(), removeAmountFinal);

        for (int i = 0; i < cardsToActOn.Count; i++)
        {
            initialDeck.Remove(cardsToActOn[i]);
        }

        cardsToActOn.Clear();
        
        for (int i = 0; i < replaceAmountFinal; i++)
        {


            HandCardSO cardToReplace = null;

            do
            {
                cardToReplace = initialDeck[GameManager.currRun.RoundRand.NextInt(0, initialDeck.Count)];

                if (cardToReplace.cardColors.Contains(favoriteColor))
                {
                    cardToReplace = initialDeck[GameManager.currRun.RoundRand.NextInt(0, initialDeck.Count)];
                }
            } while (cardsToActOn.Contains(cardToReplace));

            //Repeat drawing to create bias towards cards of not the favorite color;
            if (cardToReplace.cardColors.Contains(favoriteColor))
            {
                cardToReplace = initialDeck[GameManager.currRun.RoundRand.NextInt(0, initialDeck.Count)];
            }

            cardsToActOn.Add(cardToReplace);
        }

        for (int i = 0; i < cardsToActOn.Count; i++)
        {
            initialDeck[initialDeck.IndexOf(cardsToActOn[i], 0)] = FetchWeightedHandCard(initialDeck.Where(o => o == cardsToActOn[i]).Count());
        }

        for (int i = 0; i < addAmountFinal; i++)
        {
            initialDeck.Add(FetchWeightedHandCard(initialDeck.Where(o => o == cardsToActOn[i]).Count()));
        }

        return initialDeck;
    }

    public List<SideCardSO> GenerateSideCards(int RoundNum)
    {
        //RoundNum = 1;

        List<SideCardSO> sideCards = new List<SideCardSO>();

        for (int i = 0; i < RoundNum; i++)
        {
            sideCards.Add(FetchWeightedSideCard());
        }

        return sideCards;
    }
}
