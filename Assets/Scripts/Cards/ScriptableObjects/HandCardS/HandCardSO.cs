using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Assets.Scripts.Auxilary;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New Card")]
public class HandCardSO : CardSOBase
{
    public virtual int Score 
    { 
        get 
        {
            if (cardValues.Count > 0)
            {
                return cardValues[0] * 10;
            }
            return 0;
        }
    }

    public virtual float Mult
    {
        get => 1;
    }

    public List<Colors> cardColors;
    public List<int> cardValues;

    public bool PreWild, PostWild;

    [SerializeField]
    public virtual bool overrideValue{ get => false; }

    [SerializeField]
    public virtual bool overrideColor{ get => true; }

    [SerializeField]
    public virtual bool overrideScore{ get => false; }

    [SerializeField]
    public virtual bool overrideMult { get => false; }

    public virtual bool LockValue => false;

    public virtual bool Ignore => false;

    public virtual bool Transformer => false;

    public virtual void initCard(HandCardDataHandler card)
    {
        try
        {
            var values = Enum.GetValues(typeof(Colors));
            Colors color = (Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4));

            int number = GameManager.currRun.RoundRand.NextInt(1, 10);

            List<Colors> CardColor = overrideColor ? cardColors : new List<Colors>() { color };
            List<int> CardValue = overrideValue ? cardValues : new List<int>() { number };
            //int CardScore = overrideScore ? Score : CardValue[0] * 10;
            //float CardMult = overrideMult ? Mult : 1;
            card.SetColors(CardColor);
            card.SetValues(CardValue);
        }
        catch (Exception)
        {

            Debug.LogError(Name + " Caused some issues! Please report this as a bug :)");
            throw;
        }

    }

    public virtual void initForCollection(HandCardDataHandler card)
    {
        int number = 0;
        var values = new List<Colors>() { Colors.Red, Colors.Blue, Colors.Green, Colors.Orange };

        List<Colors> CardColor = overrideColor ? values : cardColors;
        List<int> CardValue = overrideValue ? cardValues : new List<int>() { number };
        //int CardScore = overrideScore ? Score : CardValue[0] * 10;
        //float CardMult = overrideMult ? Mult : 1;
        card.SetColors(CardColor);
        card.SetValues(CardValue);
    }

    public virtual IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator OnThisPlaced(HandCardDataHandler card)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator OnThisDrawn(HandCardDataHandler card)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        base.OnPickup(card);
    }

    public virtual bool CanActivateEffect(HandCardDataHandler card, string method)
    {
        return !card.Smoked && card.data.GetType().GetMethod(method).DeclaringType != typeof(HandCardSO);
    }

    public virtual int CalcScore(HandCardDataHandler card)
    {

        if (!overrideScore)
        {
            if (overrideValue && card.returnModifiedData().cardValues.Count == 0)
            {
                Debug.Log(name + " overrides value but doesn't have a default score. That's uh... Look into that.");
                return Score;
            }
            else
            {
                return card.returnModifiedData().cardValues[0] * 10;
            }
        }
        else
            return Score;
    }

    public virtual float CalcMult(HandCardDataHandler card)
    {
        return 1;
    }

    public virtual bool Playable(HandCardDataHandler card, HandCardDataHandler cardCompare)
    {
        if (cardCompare != null)
        {
            HandCardData TopData = cardCompare.returnTempData();
            HandCardData thisData = card.returnModifiedData();


            // ((thisData.cardValues.Intersect(TopData.cardValues).Count() > 0 || TopData.ValueWild || thisData.ValueWild)
            bool isTopValue = (TopData.cardValues.Count > 0 && thisData.cardValues.Count > 0 && (thisData.cardValues.Intersect(TopData.cardValues).Count() > 0));
            bool isTopColor = (TopData.cardColors.Count > 0 && thisData.cardColors.Count > 0 && (thisData.cardColors.Intersect(TopData.cardColors).Count() > 0));
            //bool isTopTrueWild = (TopData.ValueWild || TopData.cardColors.Count == 0) && (TopData.ColorWild || TopData.cardColors.Count == 0);
            //bool isHandTrueWild = (thisData.ValueWild || thisData.cardColors.Count == 0) && (thisData.ColorWild || thisData.cardColors.Count == 0);

            if (isTopValue || isTopColor || TopData.PostWild || thisData.PreWild) //|| isTopTrueWild || isHandTrueWild
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public override string ReturnDescription()
    {
        string raw = GameManager.instance.AssetLibrary.GetHandDescription(Name.Replace("," , ""));

        return ConvertStringToValues(raw);
    }

    public override string ReturnChoiceText()
    {
        string raw = GameManager.instance.AssetLibrary.GetHandChoiceText(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }

    public override string ReturnTriggeredText()
    {
        string raw = GameManager.instance.AssetLibrary.GetHandTrigger(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }

    public virtual string ReturnPlayedText(HandCardDataHandler card)
    {
        string raw = GameManager.instance.AssetLibrary.GetHandPlayed(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }

    public virtual string ReturnDrawnText()
    {
        string raw = GameManager.instance.AssetLibrary.GetHandDrawn(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return !(card as HandCardDataHandler).Smoked;
    }
}
