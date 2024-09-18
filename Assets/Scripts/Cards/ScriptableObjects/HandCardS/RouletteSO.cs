using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New RouletteSO")]
public class RouletteSO : XCardSO
{
    public override bool overrideColor => true;

    public override bool overrideScore => true;

    public override int Score => 150;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler> cd = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, ChooseNumber(card.owner));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        int roll = GameManager.currRun.RoundRand.NextInt(0, 10);

        if (cd.result.returnUnmodifiedData().cardValues[0] == roll)
        {
            yield return card.owner.MultScore(MultAmount);

            yield return card.TriggerCardEffect("Winner!");
        }
        else
        {
            yield return card.TriggerCardEffect("Loser...");
        }
    }
}
