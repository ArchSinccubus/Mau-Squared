using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class MasterAI : IAIBase, IObserverOnCardPlayed
{
    public AIEvaluationValues AIEvaluationValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public AIStrategy preferredStrategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Colors preferredColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public AIType AIType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<HandCardDataHandler> playedCards;

    public Dictionary<Colors, int> usedColors;

    public Dictionary<AIStrategy, int> usedStrats;

    public MasterAI()
    {
        playedCards = new List<HandCardDataHandler>();

        usedColors = new Dictionary<Colors, int>();

        for (int i = 0; i < 4; i++)
        {
            usedColors.Add((Colors)i, 0);
        }

        for (int i = 1; i < 7; i++)
        {
            usedStrats.Add((AIStrategy)i, 0);
        }

    }

    public void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
    }
    public void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
    }

    public SideCardDataHandler[] ActivateSideCards(EntityHandler owner)
    {
        List<SideCardDataHandler> ActivatedCards = owner.SideCards.Where(o => !o.Used && o.data.Clickable).ToList();

        for (int i = ActivatedCards.Count - 1; i >= 0; i--)
        {
            float removeChance = ActivatedCards[i].baseData.AIStrategyType.Contains(preferredStrategy) ? AIEvaluationValues.SideCardChanceStrat : AIEvaluationValues.SideCardChanceNoStrat;

            if (GameManager.currRun.RoundRand.NextFloat(0f, 1f) >= removeChance)
            {
                ActivatedCards.RemoveAt(i);
            }
        }

        return ActivatedCards.ToArray();
    }

    public HandCardDataHandler[] ChooseHandOption(HandCardDataHandler[] options, EntityHandler entity, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType)
    {
        List<AICardScorePair> scoringSystem = SetBaseScoreEval(options.ToList(), false);

        List<HandCardDataHandler> choices = new List<HandCardDataHandler>();

        switch (choiceType)
        {
            case HandChoiceType.Recycle:
            case HandChoiceType.Smoke:
            case HandChoiceType.Transform:
                scoringSystem = scoringSystem.OrderBy(o => o.Score).ToList();
                break;
            case HandChoiceType.Clean:
            case HandChoiceType.Upgrade:
            case HandChoiceType.PlayExtra:
                scoringSystem = scoringSystem.OrderByDescending(o => o.Score).ToList();
                break;
            case HandChoiceType.Other:
                break;
            default:
                break;
        }

        switch (mode)
        {
            case CardMenuChoiceMode.Forced:
                choices = EvalToCards(scoringSystem.Take(amount).ToList());
                break;
            case CardMenuChoiceMode.Semi:
                choices = EvalToCards(scoringSystem.Where(o => o.Score >= AIEvaluationValues.CutoffValue).ToList());
                if (choices.Count > amount)
                {
                    choices.RemoveRange(choices.Count - amount, amount + 1);
                }
                else if (choices.Count == 0)
                {
                    choices.Add(scoringSystem[0].Card);
                }
                break;
            case CardMenuChoiceMode.Open:
                choices = EvalToCards(scoringSystem.Where(o => o.Score >= AIEvaluationValues.CutoffValue).ToList());
                if (choices.Count > amount)
                {
                    choices.RemoveRange(choices.Count - amount, amount + 1);
                }
                break;
            default:
                break;
        }

        return choices.ToArray();
    }

    public HandCardDataHandler[] ChooseOption(HandCardDataHandler[] options, EntityHandler entity, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, SetChoiceType choiceType)
    {
        Dictionary<HandCardDataHandler, float> scoringSystem = new Dictionary<HandCardDataHandler, float>();

        List<HandCardDataHandler> choices = new List<HandCardDataHandler>();

        foreach (var item in options)
        {
            float score = 0;

            switch (choiceType)
            {
                case SetChoiceType.PreColor:
                case SetChoiceType.Transform:
                    if (preferredColor != Colors.None)
                    {
                        if (item.returnUnmodifiedData().cardColors.Contains(preferredColor))
                        {
                            score += AIEvaluationValues.ColorValue;
                        }
                        else
                        {
                            score += AIEvaluationValues.NoColorValue;
                        }
                    }
                    if (item.data.AIStrategyType.Count > 0)
                    {
                        if (item.data.AIStrategyType.Contains(preferredStrategy))
                        {
                            score *= AIEvaluationValues.StrategyMult;
                        }
                    }
                    break;
                case SetChoiceType.HandAdd:
                    score = GetCardScore(entity.Hand, item, false);
                    break;
                case SetChoiceType.PreValue:
                    if (item.returnUnmodifiedData().cardValues.Contains(entity.currDeck.GetMostCommonValue()))
                    {
                        score = 1;
                    }
                    break;
                case SetChoiceType.Other:
                    score = 0.5f;
                    break;
                default:
                    break;
            }

            scoringSystem.Add(item, score);
        }

        scoringSystem = scoringSystem.OrderByDescending(o => o.Value).ToDictionary(o => o.Key, o => o.Value);

        switch (mode)
        {
            case CardMenuChoiceMode.Forced:
                if (options.Length <= amount)
                {
                    choices = options.ToList();
                }
                else
                {
                    choices = scoringSystem.Keys.Take(amount).ToList();
                }
                break;
            case CardMenuChoiceMode.Semi:
                foreach (var item in scoringSystem)
                {
                    if (choices.Count < amount)
                    {
                        if (item.Value > AIEvaluationValues.CutoffValue)
                        {
                            choices.Add(item.Key);
                        }
                    }
                }

                if (choices.Count == 0)
                {
                    choices.Add(options[GameManager.currRun.RoundRand.NextInt(0, options.Length)]);
                }
                break;
            case CardMenuChoiceMode.Open:
                foreach (var item in scoringSystem)
                {
                    if (choices.Count < amount)
                    {
                        if (item.Value > AIEvaluationValues.CutoffValue)
                        {
                            choices.Add(item.Key);
                        }
                    }
                }
                break;
            default:
                break;
        }

        return choices.ToArray();
    }

    public IEnumerator ExecuteTurn(NPCHandler owner, List<HandCardDataHandler> cards)
    {
        var bestFirstCard = EvaluatePlayCard(cards);

        if (bestFirstCard != null)
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

            var bestSecondCard = EvaluatePlayCard(cards);

            if (bestFirstCard.Card.Playable)
            {
                if (bestFirstCard.Score != bestSecondCard.Score && bestFirstCard.Card != bestSecondCard.Card)
                {
                    owner.selectedCard = bestFirstCard.Score > bestSecondCard.Score ? bestFirstCard.Card : bestSecondCard.Card;
                }
                else if (bestFirstCard.Card != bestSecondCard.Card)
                {
                    float rand = GameManager.currRun.RoundRand.NextInt(0, 1);

                    owner.selectedCard = rand >= 0.5 ? bestFirstCard.Card : bestSecondCard.Card;
                }
                else
                {
                    owner.selectedCard = bestFirstCard.Card;
                }
            }
            else
            {
                owner.selectedCard = bestSecondCard.Card;
            }
        }
        else
        {
            owner.selectedCard = null;
        }
    }

    public AICardScorePair EvaluatePlayCard(List<HandCardDataHandler> cards)
    {
        List<AICardScorePair> scoringSystem = SetBaseScoreEval(cards, true);

        return scoringSystem.Where(o => o.Score > AIEvaluationValues.CutoffValue).OrderByDescending(o => o.Score).First();
    }

    private List<AICardScorePair> SetBaseScoreEval(List<HandCardDataHandler> cards, bool OnlyPlayable)
    {
        List<AICardScorePair> scoringSystem = new List<AICardScorePair>();
        foreach (var item in cards)
        {
            float score = 0;
            score = GetCardScore(cards, item, OnlyPlayable);

            scoringSystem.Add(new AICardScorePair() { Card = item, Score = score });
        }

        if (scoringSystem.Count != 0)
        {
            float min = scoringSystem.Min(o => o.Score);
            float max = scoringSystem.Max(o => o.Score);

            foreach (var item in scoringSystem)
            {
                item.Score = (item.Score - min) / (max - min);

                if (min == max)
                {
                    item.Score = 1;
                }
            }
        }

        return scoringSystem;

    }

    private float GetCardScore(List<HandCardDataHandler> cards, HandCardDataHandler item, bool checkPlayable)
    {
        float OpponentColors = (4 - item.returnModifiedData().cardColors.Intersect(CalculateLikelyColor()).Count()) / 10;

        //float OpponentStrat = (6 - item.baseData.AIStrategyType.Intersect(CalculateLikelyStrat()).Count()) / 10;

        float score = (float)Math.Pow((int)item.baseData.Rarity * AIEvaluationValues.RarityMult, 2);

        if ((item.returnModifiedData().cardColors.Contains(preferredColor) || 
            item.returnModifiedData().cardColors.Contains(item.owner.currDeck.GetMostCommonColor())) 
            && preferredColor != Colors.None)
        {
            score += AIEvaluationValues.ColorValue;
        }
        else
        {
            score -= AIEvaluationValues.NoColorValue;
        }

        score += OpponentColors;

        score += item.ReturnCalcScore() / 100f;

        foreach (var card in cards)
        {
            if (card.returnModifiedData().cardValues.Intersect(item.returnModifiedData().cardValues).Count() > 0 || card.returnModifiedData().cardValues.Contains(card.owner.currDeck.GetMostCommonValue()))
            {
                score += AIEvaluationValues.CardValueValue;
            }
        }

        if ((item.baseData.AIStrategyType.Contains(preferredStrategy)) && preferredStrategy != AIStrategy.None)
        {
            score *= (AIEvaluationValues.StrategyMult);
        }

        if (checkPlayable && !item.Playable)
            score = 0;

        return (float)Math.Pow(Math.Abs(score), 2.5f) * Math.Sign(score);
    }


    public List<Colors> CalculateLikelyColor()
    {
        List<Colors> mostUsedColors = new List<Colors>();

        int max = usedColors.Values.Max();

        foreach (var item in usedColors)
        {
            if (item.Value == max)
            {
                mostUsedColors.Add(item.Key);
            }
        }

        return mostUsedColors;
    }

    public List<AIStrategy> CalculateLikelyStrat()
    {
        List<AIStrategy> mostUsedStrats = new List<AIStrategy>();

        int max = usedStrats.Values.Max();

        foreach (var item in usedStrats)
        {
            if (item.Value == max)
            {
                mostUsedStrats.Add(item.Key);
            }
        }

        return mostUsedStrats;
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        HandCardDataHandler card = args.Data as HandCardDataHandler;

        playedCards.Add(card);

        if (card.returnTempData().cardColors.Any() && card.owner == GameManager.currRun.player)
        {
            foreach (var c in card.returnTempData().cardColors)
            {
                if (c != Colors.None)
                {
                    usedColors[c]++;
                }
            }
        }

        foreach (var s in card.baseData.AIStrategyType)
        {
            usedStrats[s]++;
        }

        yield return new WaitForGameEndOfFrame();
    }

    public List<HandCardDataHandler> EvalToCards(List<AICardScorePair> cards)
    {
        List<HandCardDataHandler> rawCards = new List<HandCardDataHandler>();

        foreach (var item in cards)
        {
            rawCards.Add(item.Card);
        }

        return rawCards;
    }
}