using Assets.Scripts.Auxilary;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New AdventureSO")]
public class AdventureSO : PlusSO
{
    public override bool ChoiceCard => true;

    public override bool overrideScore => true;

    public override int Score => 100;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler> cd = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, ChooseColor(card.owner));
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        Colors c = cd.result.returnUnmodifiedData().cardColors[0];

        switch (c)
        {
            case Colors.Red:
                card.SetTempMultScore(MultAmount);
                break;
            case Colors.Orange:
                yield return card.owner.AddMoney(card.owner.Hand.Count * MoneyAmount);
                break;
            case Colors.Blue:
                List<HandCardDataHandler> cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(target.Hand.Where(c => !c.Smoked).ToList(), NumberAmount);
                yield return target.SmokeCards(card.owner ,cardsToSmoke.ToArray());
                break;
            case Colors.Green:
                CoroutineWithData<HandCardDataHandler[]> cd2 = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => o.data.Playable(o, card), HandChoiceType.PlayExtra));

                yield return cd2.coroutine;

                while (cd2.result == null)
                {
                    yield return new WaitForGameEndOfFrame();
                }

                if (cd2.result.Length > 0)
                {
                    card.TempData1 = cd2.result;
                }
                break;
            default:
                break;
        }

    }

    public IEnumerator ChooseColor(EntityHandler entity)
    {

        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        for (int i = 0; i < 4; i++)
        {
            Colors c = (Colors)i;

            HandCardDataHandler tempCard = new HandCardDataHandler(GameManager.instance.AssetLibrary.FetchHandCardTemplate(), null, true);

            tempCard.SetMainColor(c);
            tempCard.SetMainValue(0);
            tempCard.SetScore(0);

            list.Add(tempCard);
        }

        list[0].CustomToolTip = ReturnChoiceDescription("R");
        list[1].CustomToolTip = ReturnChoiceDescription("O");
        list[2].CustomToolTip = ReturnChoiceDescription("B");
        list[3].CustomToolTip = ReturnChoiceDescription("G");

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.ToArray(), ReturnChoiceDescription("BaseChoice"), 1, CardMenuChoiceMode.Forced, SetChoiceType.Other));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return cd.result[0];
        }
    }

    public string ReturnChoiceDescription(string card)
    {
        JToken token = GameManager.instance.AssetLibrary.GetAdventureTimeStuff(Name);

        return token[card].ToString().Replace("+{}", "+" + ScoreAmount).Replace("x{}", "x" + MultAmount).Replace("c{}", ChoiceAmount.ToString()).Replace("m{}", MoneyAmount.ToString()).Replace("n{}", NumberAmount.ToString());

    }
}
