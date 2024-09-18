using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;
using Assets.Scripts.Auxilary;

public class BaseCardDataHandler
{
    public Int64 ID;

    [JsonIgnore]
    public ICardVisuals visuals;

    [JsonIgnore]
    public CardSOBase baseData;

    [JsonIgnore]
    public EntityHandler owner;

    public bool temp;

    public int price;

    [JsonIgnore]
    protected ChoiceDelegate choice;

    public string CustomToolTip;

    //Used for temporary data from SO effects
    public object TempData1;
    public object TempData2;

    public object PermData1;
    public bool TempBool;


    public virtual bool PlayerControl { get; set; }

    public BaseCardDataHandler(CardSOBase newData, EntityHandler owner, bool temp)
    {
        ID = GameManager.returnID();

        baseData = newData;

        this.owner = owner;
        this.temp = temp;
        this.price = newData.BasePrice;

        TempData1 = null;
        TempData2 = null;
    }

    public BaseCardDataHandler(CardSOBase newData) 
    {
        ID = GameManager.returnID();

        baseData = newData;

        this.owner = null;
        this.temp = true;
        this.price = newData.BasePrice;

        TempData1 = null;
        TempData2 = null;
    }

    public BaseCardDataHandler(BaseCardDataHandler copy)
    {
        ID = GameManager.returnID();


        baseData = copy.baseData;
        this.owner = copy.owner;
        this.temp = copy.temp;

        TempData1 = copy.TempData1;
        TempData2 = copy.TempData2;
        TempBool = copy.TempBool; 

    }

    [JsonConstructor]
    public BaseCardDataHandler(long iD, EntityHandler owner, bool temp, object tempData1, object tempData2, object permData1, bool tempBool)
    {
        ID = iD;
        this.owner = owner;
        this.temp = temp;
        TempData1 = tempData1;
        TempData2 = tempData2;
        PermData1 = permData1;
        TempBool = tempBool;
    }

    public virtual void InitForRound(bool highlight)
    {
        
    }

    public virtual void InitForChoice(bool highlight, ChoiceDelegate del, int Canvasoverride)
    { 
    
    }

    public virtual IEnumerator TriggerCardEffect()
    {
        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator TriggerCardEffect(string text)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator TriggerCardText(string text)
    { 
        yield return visuals.PopText(text);
    }

    public void changeChoice(ChoiceDelegate del)
    {
        choice = del;
    }

    public virtual void InitForView(bool side, int Canvasoverride, bool Col)
    { 
        InitVisuals(side, baseData, true, Canvasoverride, Col);
    }

    public virtual void InitVisuals(bool side, CardSOBase data, bool highlight, int Canvasoverride, bool Col)
    {
        if (visuals == null)
        {
            ICardVisuals vis = PoolingManager.DrawFromPool(side, Col);
            vis.init(data, highlight, data.SideCard, Canvasoverride);
            visuals = vis;
        }
        visuals.OnToolTip = ShowToolTip;
    }

    public virtual void Deload()
    {
        PoolingManager.ReturnToPool(visuals);

        this.visuals = null;
    }

    public virtual HandCardData returnModifiedData()
    {
        return new HandCardData();
    }

    public virtual HandCardData returnUnmodifiedData()
    {
        return new HandCardData();
    }

    public virtual void ClearForRound()
    {
        choice = null;
        if (baseData.RoundEvents)
        {
            baseData.Unsubscribe(baseData);
            TempData1 = null;
            TempData2 = null;
            TempBool = false;
        }

        if (temp || !owner.IsPlayer)
        {
            Deload();
        }
    }

    public virtual string ReturnCardText()
    {
        return "";
    }

    public virtual void ShowToolTip()
    { 
        
    }

    public virtual void HideCard()
    {
        visuals.HideCard();
        visuals.OnToolTip = null;
    }

    public virtual void RevealCard()
    {
        visuals.RevealCard();
        visuals.OnToolTip = ShowToolTip;
    }
}
