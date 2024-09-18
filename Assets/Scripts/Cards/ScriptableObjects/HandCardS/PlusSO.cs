using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PlusSO")]
public class PlusSO : HandChoiceBaseSO
{
    public bool trigger;

    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override int Score => 50;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => o.data.Playable(o, card), HandChoiceType.PlayExtra));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }
        if (cd.result.Length > 0)
        {
            card.TempData1 = cd.result;
        }
    }

    public override IEnumerator OnThisPlaced(HandCardDataHandler card)
    {
        if (card.TempData1 != null)
        {
            HandCardDataHandler[] cardsToPlay = card.TempData1 as HandCardDataHandler[];

            for (int i = cardsToPlay.Length - 1; i >= 0; i--)
            {
                if (trigger)
                {
                    yield return cardsToPlay[i].PlayCard();
                }
                else
                {
                    yield return cardsToPlay[i].PlayCardNoTrigger();
                }
            }
        }
    }

}
