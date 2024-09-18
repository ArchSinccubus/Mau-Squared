using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New LootingCardSO")]
public class LootingCardSO : HandCardSO
{


    public override int Score => 20;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        HandCardDataHandler opCard = GameManager.currRun.RoundRand.GetRandomElements(target.Hand.Where(c => !c.Smoked).ToList(), NumberAmount)[0];
        HandCardDataHandler ownerCard = GameManager.currRun.RoundRand.GetRandomElements(card.owner.Hand.Where(c => !c.Smoked).ToList(), NumberAmount)[0];

        CoroutineWaitForList list = new CoroutineWaitForList();

        GameManager.Round.StartCoroutine(list.CountCoroutine(target.RecycleCards(opCard)));
        GameManager.Round.StartCoroutine(list.CountCoroutine(card.owner.RecycleCards(ownerCard)));

        yield return list;
    }
}
