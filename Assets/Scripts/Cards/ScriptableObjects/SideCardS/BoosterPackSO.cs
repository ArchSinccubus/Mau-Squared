using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New BoosterPackSO")]
public class BoosterPackSO : SideCardSO
{
    public override bool ChoiceCard => true;

    public string choiceText;
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, ChooseCard(card.owner));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            cd.result[0].InitForRound(card.owner.IsPlayer);

            yield return card.owner.AddHandCard(cd.result[0]);
        }
    }

    public IEnumerator ChooseCard(EntityHandler entity)
    {

        HandCardSO[] list = GameManager.instance.AssetLibrary.FetchRandomHandCards(GameManager.currRun.RoundRand, NumberAmount);
        List<HandCardDataHandler> choices = new List<HandCardDataHandler>();

        for (int i = 0; i < list.Length; i++)
        {
            choices.Add(new HandCardDataHandler(list[i], entity, true));
        }

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(choices.ToArray(), choiceText, ChoiceAmount, CardMenuChoiceMode.Open, SetChoiceType.HandAdd));

        yield return cd.coroutine;


        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return cd.result;
        
        
    }
}
