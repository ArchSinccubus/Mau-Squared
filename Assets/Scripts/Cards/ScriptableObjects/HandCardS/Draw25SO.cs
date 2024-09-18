using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New Draw25SO")]
public class Draw25SO : HandCardSO
{
    public override bool overrideScore => true;

    public override bool overrideValue => true;

    public override bool overrideColor => true;

    public override int Score => 250;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        yield return target.Draw(NumberAmount);

        card.owner.currDeck.DeckBase.Remove(card);

        yield return card.visuals.Shake();
    }
}
