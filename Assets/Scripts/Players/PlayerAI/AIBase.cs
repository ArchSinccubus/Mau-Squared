using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For information sake, not if else loops in the code
/// <summary>
/// Simple explenation of what each AI type is supposed to do:
/// Random - All random choices. No brains whatsoever
/// Dumb - Will at least think one or two steps ahead, but no further. Example, will not play a card that changes the color if they have more of that color in hand to play.
/// Smart - Balanced smartness. Will try and gather a good combo going, able to react to things!
/// Careful - Will save reaction cards to counter the player more often than not.
/// Aggressive - Will play as much cards as possible at all times, no matter the cost. Will never skip a turn stratigically
/// Combo - Will look to play many cards at the same time. Will prefer that to just rushing down.
/// </summary>

public interface IAIBase
{
    public AIEvaluationValues AIEvaluationValues { get; set; }
    AIStrategy preferredStrategy { get; set; }
    Colors preferredColor { get; set; }

    AIType AIType { get; set; }

    //public KeyValuePair<HandCardDataHandler, float> EvaluatePlayCard(List<HandCardDataHandler> cards);

    public HandCardDataHandler[] ChooseOption(HandCardDataHandler[] options,EntityHandler entity, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, SetChoiceType choiceType);

    public HandCardDataHandler[] ChooseHandOption(HandCardDataHandler[] options, EntityHandler entity, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType);

    public SideCardDataHandler[] ActivateSideCards(EntityHandler owner);

    public IEnumerator ExecuteTurn(NPCHandler owner, List<HandCardDataHandler> cards);

}
