using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New HandChoiceSideSO")]
public class HandChoiceSideSO : SideCardSO
{
    public override bool ChoiceCard => true;

    public string choiceText;

    public IEnumerator GetExtraCards(EntityHandler entity, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeHandChoice(choiceText, ChoiceAmount, mode, query, choiceType));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return cd.result;
    }
}
