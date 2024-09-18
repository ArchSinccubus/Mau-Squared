using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New StarSO")]
public class StarSO : SideCardSO
{
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        yield return card.owner.Draw(1);

        if (card.owner.Hand[card.owner.Hand.Count - 1].Playable)
        {
            yield return card.owner.Hand[card.owner.Hand.Count - 1].PlayCard();
            card.owner.UpdateHand();
        }

    }
}
