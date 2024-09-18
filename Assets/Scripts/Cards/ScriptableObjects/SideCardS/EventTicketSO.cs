using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New EventTicketSO")]
public class EventTicketSO : SideCardSO
{
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        var newData = GameManager.instance.AssetLibrary.FetchRandomHandCard(GameManager.currRun.RoundRand, o => o.Rarity == CardRarity.Rare);

        HandCardDataHandler newCard = new HandCardDataHandler(newData, card.owner, true);

        newCard.InitForRound(card.owner.IsPlayer);

        yield return card.owner.AddHandCard(newCard);
    }
}
