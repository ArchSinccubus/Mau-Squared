using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Change Color Card", menuName = "Mau/Cards/Hand/New ChangeColorCard")]
public class ChangeColorCardSO : HandCardSO
{
    public string choiceText;
    public override bool ChoiceCard => true;

    public override bool overrideColor => true;
    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override int Score => 100;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler> cd = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, ChooseColor(card.owner));

        yield return cd.coroutine;

        while (cd.result == null) 
        {
            yield return new WaitForGameEndOfFrame();
        }

        Debug.Log(cd.result.ReturnMainColor() + " was chosen!");

        yield return card.visuals.Flip(false);

        card.SetTempMainColor(cd.result.ReturnMainColor());

        yield return card.visuals.Flip(true);

        card.SetTempNotPostWild();

    }

    public IEnumerator ChooseColor(EntityHandler entity)
    {

        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        for (int i = 0; i < 4; i++)
        {
            Colors c = (Colors)i;

            HandCardDataHandler tempCard = new HandCardDataHandler(GameManager.instance.AssetLibrary.FetchHandCardTemplate(), null, true);

            tempCard.SetMainColor(c);
            tempCard.SetTempMainValue(0);

            list.Add(tempCard);
        }

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.ToArray(), choiceText, 1, CardMenuChoiceMode.Forced, SetChoiceType.Transform));

        yield return cd.coroutine;


        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return cd.result[0];
    }

    public override string ReturnPlayedText(HandCardDataHandler card)
    {
        if (card.returnTempData().cardColors.Any())
        {
            return card.returnTempData().cardColors[0].ToString();
        }
        return "";
    }
}
