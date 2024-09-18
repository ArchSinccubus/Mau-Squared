using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New KillTowerSO")]
public class KillTowerSO : HandCardSO
{
    public override bool overrideColor => true;

    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override int Score => 20;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (var item in GameManager.Round.Pile)
        {
            GameManager.Round.StartCoroutine(list.CountCoroutine(item.visuals.Wiggle()));
            GameManager.Round.StartCoroutine(list.CountCoroutine(card.visuals.Pop()));

            card.SetTempMultScore(MultAmount);

            yield return list;
        }
    }



}
