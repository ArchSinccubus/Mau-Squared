using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new Hand Size Modifier" , menuName = "Mau/Cards/Side/New HandSizeModCard")]
public class SleeveCardSO : SideCardSO, IPlayerDecorator
{
    public PlayerData Decorate(PlayerData player)
    {
        player.StartHandSize += NumberAmount;

        return player;
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.playerDecorators.Add(this);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.playerDecorators.Remove(this);

        base.OnRemove(card);
    }
}