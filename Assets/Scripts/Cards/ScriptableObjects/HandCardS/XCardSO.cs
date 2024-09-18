using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New XCardSO")]
public class XCardSO : HandCardSO
{
    public override bool ChoiceCard => true;

    public override bool overrideColor => false;
    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public string choiceText;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler> cd = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, ChooseNumber(card.owner));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return FlipCard(card, cd.result.returnUnmodifiedData().cardValues[0]);
        card.SetTempNotPreWild();
    }

    public IEnumerator ChooseNumber(EntityHandler entity)
    {

        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        for (int i = 1; i <= 9; i++)
        {
            HandCardDataHandler tempCard = new HandCardDataHandler(GameManager.instance.AssetLibrary.FetchHandCardTemplate(), null, true);

            tempCard.SetMainValue(i);
            tempCard.SetTempMainValue(i);

            list.Add(tempCard);
        }

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.ToArray(), choiceText, 1, CardMenuChoiceMode.Forced, SetChoiceType.Transform));

        yield return cd.coroutine;


        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            Debug.Log("Happened");

            yield return cd.result[0];
        }
    }

    public IEnumerator FlipCard(HandCardDataHandler card, int value)
    {
        yield return card.visuals.Flip(false);

        card.SetTempMainValue(value);
        card.SetScore(value * 10);

        yield return card.visuals.Flip(card.owner.IsPlayer);
    }

    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }

        if (card.returnModifiedData().cardValues.Count > 0)
        {
            return base.CalcScore(card);
        }
        return 0;
    }
}
