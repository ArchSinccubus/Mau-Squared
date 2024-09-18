using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Experimental.Rendering;

namespace Assets.Scripts.Players.PlayerAI
{
    internal class AverageAI : IAIBase
    {
        public AIEvaluationValues AIEvaluationValues { get; set; }

        public AIStrategy preferredStrategy { get; set; }
        public Colors preferredColor { get; set; }
        public AIType AIType { get; set; }

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
                                score = 1;
                            }
                        }
                        break;
                    case SetChoiceType.HandAdd:
                        score = GetCardScore(entity.Hand, item, false);
                        break;
                    case SetChoiceType.PreValue:
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

        public AICardScorePair EvaluatePlayCard(List<HandCardDataHandler> cards)
        {
            List<AICardScorePair> scoringSystem = SetBaseScoreEval(cards, true);

            if (scoringSystem.Count > 0)
            {
                scoringSystem = scoringSystem.Where(o => o.Score > AIEvaluationValues.CutoffValue).OrderByDescending(o => o.Score).ToList();

                float total = scoringSystem.Sum(o => o.Score);

                float rand = GameManager.currRun.RoundRand.NextFloat(0, total);
                int ID = scoringSystem.Count - 1;

                while (rand - scoringSystem.ToArray()[ID].Score > 0)
                {
                    rand -= scoringSystem.ToArray()[ID].Score;
                    ID--;
                }

                return scoringSystem[ID];
            }

            return null;
        }


        public SideCardDataHandler[] ActivateSideCards(EntityHandler owner)
        {
            List<SideCardDataHandler> ActivatedCards = owner.SideCards.Where(o => !o.Used && o.data.Clickable).ToList();

            for (int i = ActivatedCards.Count - 1; i >=0; i--)
            {
                float removeChance = ActivatedCards[i].baseData.AIStrategyType.Contains(preferredStrategy) ? AIEvaluationValues.SideCardChanceStrat : AIEvaluationValues.SideCardChanceNoStrat;

                if (GameManager.currRun.RoundRand.NextFloat(0f, 1f) >= removeChance)
                { 
                    ActivatedCards.RemoveAt(i);
                }
            }

            return ActivatedCards.ToArray();
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
            float score = (int)item.baseData.Rarity * AIEvaluationValues.RarityMult;
            if (item.returnModifiedData().cardColors.Contains(preferredColor) && preferredColor != Colors.None)
            {
                score += AIEvaluationValues.ColorValue;
            }
            else
            {
                score -= AIEvaluationValues.NoColorValue;
            }

            score += item.ReturnCalcScore() / 100f;

            foreach (var card in cards)
            {
                if (card.returnModifiedData().cardValues.Intersect(item.returnModifiedData().cardValues).Count() > 0)
                {
                    score += AIEvaluationValues.CardValueValue;
                }
            }

            if (item.baseData.AIStrategyType.Contains(preferredStrategy) && preferredStrategy != AIStrategy.None)
            {
                score *= AIEvaluationValues.StrategyMult;
            }
            else
            {
                score *= AIEvaluationValues.NoStrategyMult;
            }

            if (checkPlayable && !item.Playable)
                score = 0;

            return score;
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

                float mod = bestFirstCard.Score + bestSecondCard.Score;
                if (bestFirstCard.Card.Playable)
                {
                    if (bestFirstCard.Card != bestSecondCard.Card)
                    {
                        float rand = GameManager.currRun.RoundRand.NextFloat(0, mod);

                        owner.selectedCard = bestFirstCard.Score > rand ? bestFirstCard.Card : bestSecondCard.Card;
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
}
