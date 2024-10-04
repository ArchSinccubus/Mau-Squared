using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New WheelFortuneSO")]
public class WheelFortuneSO : SideCardSO
{
    public override bool Clickable => true;

    public override bool Tarot => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        HandCardSO newCardSO = GameManager.instance.AssetLibrary.FetchRandomHandCard(GameManager.currRun.RoundRand, o => o.Tarot);

        HandCardDataHandler newCard = new HandCardDataHandler(newCardSO, card.owner, true);

        yield return card.owner.AddHandCard(newCard);
    }
}
