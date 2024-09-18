using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New LibraryCardSO")]
public class LibraryCardSO : HandChoiceSideSO
{
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => true, HandChoiceType.Recycle));

        card.TempBool = true;
        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return card.owner.RecycleCards(cd.result);
        }
    }
}
