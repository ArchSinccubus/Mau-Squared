using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class RoundVisualManager : MonoBehaviour
{
    public float maxRad;

    public EntityVisualManager[] players;

    public Transform PileLoc;

    public TextMeshProUGUI TurnText;

    public ScreenMoverHelper mover;

    [SerializeField]
    private TextMeshProUGUI TurnIndicator;
    [SerializeField]
    private Transform TurnStart, TurnStop, TurnEnd;

    [SerializeField]
    private TextMeshProUGUI AnteText;

    public void Init(int Ante)
    {
        AnteText.text = Ante.ToString() + "/$";
    }

    public IEnumerator StartNewTurn(int num)
    {
        TurnText.text = "Turn: " + num.ToString();

        yield return new WaitForGameEndOfFrame();
    }

    public IEnumerator MoveCardToPile(ICardVisuals card)
    {
        float dist = GameManager.currRun.RoundRand.NextFloat(0f, maxRad * CanvasScaleFitter.CurrScale);
        float angle = GameManager.currRun.RoundRand.NextFloat(0f, 360f);


        Vector3 vec = Quaternion.Euler(0, 0, angle) * Vector2.up;
        vec *= dist;
        vec.z = 0;

        float cardAngle = AngleBetweenPoints(card.GetPos(), PileLoc.position + vec);

        card.transform.localRotation = Quaternion.Euler(0, 0, cardAngle + 90);

        yield return card.MoveCard(PileLoc, PileLoc.position + vec, "toss");
    }

    public IEnumerator ShowTurnChange(int num)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["toss"];
        TurnIndicator.text = "Turn " + num + "!";

        TurnIndicator.transform.position = TurnStart.position;

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            TurnIndicator.transform.position = Vector3.Lerp(TurnStart.position, TurnStop.position, curve.returnValue(time * GameManager.GetTimeSpeed()));
            time += Time.deltaTime;
            yield return new WaitForGameEndOfFrame();
        }

        TurnIndicator.transform.position = TurnStop.position;

        yield return new WaitForGameSeconds(1f / GameManager.GetTimeSpeed());

        time = 0;

        while (time < curve.returnTotalTime())
        {
            TurnIndicator.transform.position = Vector3.Lerp(TurnStop.position, TurnEnd.position, curve.returnValue(time * GameManager.GetTimeSpeed()));
            time += Time.deltaTime;
            yield return new WaitForGameEndOfFrame();
        }

        TurnIndicator.transform.position = TurnEnd.position;
    }

    public void PutCardToPile(ICardVisuals card, Vector2 position, Quaternion rotation)
    {
        card.transform.SetParent(PileLoc);
        card.transform.position = position;
        card.transform.rotation = rotation;
    }

    float AngleBetweenPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    public void Deload()
    {
        foreach (var item in players)
        {
            item.HandLoc.RemoveCards();
        }
    }
}
