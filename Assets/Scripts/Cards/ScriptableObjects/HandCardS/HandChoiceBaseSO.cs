using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HandChoiceBaseSO")]
public class HandChoiceBaseSO : HandCardSO
{
    public override bool ChoiceCard => true;

    public IEnumerator GetExtraCards(EntityHandler entity, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeHandChoice(ReturnChoiceText(), ChoiceAmount, mode, query, choiceType));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return cd.result;
    }
}
