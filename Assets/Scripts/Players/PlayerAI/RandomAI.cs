using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomAI : IAIBase
{
    public AIEvaluationValues AIEvaluationValues { get; set; }
    public AIStrategy preferredStrategy { get; set; }
    public Colors preferredColor { get; set; }
    public AIType AIType { get; set; }

    public RandomAI()
    { 
    
    }

    [JsonConstructor]
    public RandomAI(AIEvaluationValues aIEvaluationValues, AIStrategy preferredStrategy, Colors preferredColor, AIType aIType)
    {
        AIEvaluationValues = aIEvaluationValues;
        this.preferredStrategy = preferredStrategy;
        this.preferredColor = preferredColor;
        AIType = aIType;
    }

    public HandCardDataHandler[] ChooseOption(HandCardDataHandler[] options, EntityHandler entity, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, SetChoiceType choiceType)
    {
        HandCardDataHandler[] finalOptions = options;

        if (query != null)
        {
            finalOptions = options.Where(o => query(o)).ToArray();
        }

        if (mode == CardMenuChoiceMode.Open)
        {
            if (GameManager.currRun.RoundRand.NextFloat(0f, 1f) > 0.5f)
                return new HandCardDataHandler[0];
        }

        int actAmount = mode == CardMenuChoiceMode.Semi ? GameManager.currRun.RoundRand.NextInt(0, amount): amount;

        return GameManager.currRun.RoundRand.GetRandomElements(options.ToList(), actAmount).ToArray();              
    }

    public HandCardDataHandler[] ChooseHandOption(HandCardDataHandler[] options, EntityHandler entity, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType)
    {
        return ChooseOption(options,entity, amount, mode, query, SetChoiceType.HandAdd);
    }

    public HandCardDataHandler EvaluatePlayCard(List<HandCardDataHandler> cards)
    {
        List<HandCardDataHandler> availableCards = cards.FindAll(o => o.Playable);

        if (availableCards.Count > 0) 
        {
            return availableCards[GameManager.currRun.RoundRand.NextInt(0, availableCards.Count)];
        }

        return null;
    }

    public SideCardDataHandler[] ActivateSideCards(List<SideCardDataHandler> cards, EntityHandler owner)
    {
        return null;
    }

    public SideCardDataHandler[] ActivateSideCards(EntityHandler owner)
    {
        List<SideCardDataHandler> ActivatedCards = owner.SideCards.Where(o => !o.Used && o.data.Clickable).ToList();

        for (int i = ActivatedCards.Count - 1; i >= 0; i--)
        {
            if (GameManager.currRun.RoundRand.NextFloat(0f, 1f) > 0.5f)
            {
                ActivatedCards.RemoveAt(i);
            }
        }

        return ActivatedCards.ToArray();
    }

    public IEnumerator ExecuteTurn(NPCHandler owner, List<HandCardDataHandler> cards)
    {
        SideCardDataHandler[] sides = ActivateSideCards(owner);

        foreach (var item in sides)
        {
            owner.ActivatedEffects = true;
            owner.CanAct = false;
            yield return item.CommandCoroutine();
            item.Used = true;
        }

        owner.UpdateHand();
        HandCardDataHandler bestFirstCard = EvaluatePlayCard(cards);

        float rand = GameManager.currRun.RoundRand.NextFloat(0f, 1f);

        if (rand > 0.2)
        {
            owner.selectedCard = bestFirstCard;
        }
    }
}
