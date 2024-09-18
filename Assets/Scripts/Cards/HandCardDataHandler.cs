using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Accessibility;
using static UnityEngine.Rendering.DebugUI;


public delegate void ChoiceDelegate(BaseCardDataHandler card);

public class HandCardDataHandler : BaseCardDataHandler
{
    [JsonProperty]
    private HandCardData cardData;

    [JsonProperty]
    private HandCardData tempCardData;

    private HandCardData CurrData 
    {
        get 
        {
            if (temp)
            {
                return cardData;
            }
            else if (GameManager.currRun.runState == GameState.InRound)
            {
                return tempCardData;
            }

            return cardData;
        }
    }

    [JsonIgnore]
    public HandCardSO data;

    public bool Playable;

    public bool Visible;

    public bool Peeking;

    public HandCardState state;

    public bool ignore;

    private bool playerControl;

    private bool TempBase, TempMult;

    [JsonProperty]
    private bool smoked;
    
    [JsonIgnore]
    public bool Smoked 
    { 
        get { return smoked; }
        set
        {
            smoked = value;
            if (visuals != null)
            {
                visuals.SetSmoke(value);
            }
        }
    }

    [JsonIgnore]
    public override bool PlayerControl { get { return playerControl; }
        set
        { playerControl = value;

            if (value)
            {
                visuals.EnableCardForPlayer();
                RevealCard();
            }
            if (!value)
            {
                visuals.DisableCardForPlayer();
                HideCard();
            }
        }
        
    }

    #region Inits
    public HandCardDataHandler(HandCardSO newData, EntityHandler owner, bool temp) : base(newData, owner, temp)
    {
        CustomToolTip = "";
        cardData = new HandCardData();
        tempCardData = new HandCardData();

        cardData.cardColors = new List<Colors>();
        cardData.cardValues = new List<int>();

        tempCardData.cardColors = new List<Colors>();
        tempCardData.cardValues = new List<int>();

        cardData.PreWild = newData.PreWild;
        cardData.PostWild = newData.PostWild;

        tempCardData.PreWild = cardData.PreWild;
        tempCardData.PostWild = cardData.PostWild;

        cardData.Mult = newData.Mult;
        tempCardData.Mult = newData.Mult;

        cardData.Name = newData.Name;
        tempCardData.Name = newData.Name;

        data = newData;

        data.initCard(this);

        ignore = data.Ignore;

        TempBase = TempMult = false;

        this.owner = owner;
    }

    //For presenting in Collections Menu
    public HandCardDataHandler(HandCardSO newData) : base(newData)
    {
        CustomToolTip = "";
        cardData = new HandCardData();
        tempCardData = new HandCardData();

        cardData.cardColors = new List<Colors>();
        cardData.cardValues = new List<int>();

        tempCardData.cardColors = new List<Colors>();
        tempCardData.cardValues = new List<int>();

        cardData.PreWild = newData.PreWild;
        cardData.PostWild = newData.PostWild;

        tempCardData.PreWild = cardData.PreWild;
        tempCardData.PostWild = cardData.PostWild;

        cardData.Mult = newData.Mult;
        tempCardData.Mult = newData.Mult;

        cardData.Name = newData.Name;
        tempCardData.Name = newData.Name;

        data = newData;

        data.initForCollection(this);

        ignore = data.Ignore;

        TempBase = TempMult = false;
    }

    public HandCardDataHandler(HandCardDataHandler copy) : base(copy)
    {
        CustomToolTip = copy.CustomToolTip;
        cardData = copy.cardData;

        cardData.PreWild = copy.returnUnmodifiedData().PreWild;
        cardData.PostWild = copy.returnUnmodifiedData().PostWild;

        tempCardData = copy.tempCardData;

        data = copy.data;

        ignore = copy.ignore;
        smoked = copy.smoked;

        TempBool = copy.TempBool;
        TempData1 = copy.TempData1;
        TempData2 = copy.TempData2;
        PermData1 = copy.PermData1;

        TempBase = copy.TempBase;
        TempMult = copy.TempMult;

        this.owner = copy.owner;

        if (!baseData)
        {
            baseData = data;
        }
    }

    [JsonConstructor]
    public HandCardDataHandler(long ID, EntityHandler owner, HandCardData cardData, HandCardData tempCardData, HandCardSO data, bool playable, bool visible, 
        bool peeking, HandCardState state, bool ignore, bool playerControl, bool smoked, bool temp, object tempData1, object tempData2, object permData1, bool tempBool, bool TempBase, bool TempMult) : 
        base(ID, owner,temp, tempData1,tempData2, permData1, tempBool)
    {
        this.cardData = cardData;
        this.tempCardData = tempCardData;
        baseData = GameManager.instance.AssetLibrary.FetchHandCard(cardData.Name.Replace("'", ""));
        this.data = GameManager.instance.AssetLibrary.FetchHandCard(tempCardData.Name.Replace("'", ""));
        Playable = playable;
        Visible = visible;
        Peeking = peeking;
        this.state = state;
        this.ignore = ignore;
        Smoked = smoked;
        this.TempBase = TempBase;
        this.TempMult = TempMult;

        
    }

    public override void InitForRound(bool highlight)
    {
        InitVisuals(false, data, highlight, 0, false);
        Visible = false;

        visuals.UpdateCard(cardData.cardColors, ReturnCardText());
        if (owner.IsPlayer)
        {
            visuals.OnClickedEvent = SelectForPlay;
        }
        if (data.RoundEvents)
        {
            data.Subscribe(this);
        }

        TempBase = TempMult = false;
    }

    public void InitForPile()
    {
        InitVisuals(false, data, false, 0, false);
        Visible = false;

        visuals.UpdateCard(tempCardData.cardColors, ReturnCardText());
        visuals.DisableCardForPlayer();
        visuals.RevealCard();

        if (data.RoundEvents)
        {
            data.Subscribe(this);
        }
    }

    public override void InitForView(bool side, int CanvasOverride, bool Col)
    {
        base.InitForView(side, CanvasOverride, Col);

        visuals.SetSmoke(smoked);
    }

    public override void InitForChoice(bool highlight, ChoiceDelegate del, int CanvasOverride)
    {
        InitVisuals(false, data, highlight, CanvasOverride, false);
        Visible = false;

        visuals.UpdateCard(returnUnmodifiedData().cardColors, ReturnCardText());

        visuals.OnClickedEvent = SelectForChoice;

        choice = del;
    }

    public override void InitVisuals(bool side, CardSOBase data, bool highlight, int Canvasoverride, bool Col)
    {
        if (visuals == null)
        {
            ICardVisuals vis = PoolingManager.DrawFromPool(side, Col);
            vis.init(data, highlight, data.SideCard, Canvasoverride);
            vis.SetShine(CurrData.PreWild);
            visuals = vis;
        }
        visuals.OnToolTip = ShowToolTip;
    }

    public override void ClearForRound()
    {
        if (tempCardData.Name != cardData.Name)
        {
            data = GameManager.instance.AssetLibrary.FetchHandCard(cardData.Name);
        }


        tempCardData = new HandCardData()
        {
            cardColors = cardData.cardColors.ToList(),
            cardValues = cardData.cardValues.ToList(),
            PreWild = cardData.PreWild,
            PostWild = cardData.PostWild,
            Score = cardData.Score,
            Mult = cardData.Mult,
            Name = cardData.Name
        };

        choice = null;

        Smoked = false;

        state = HandCardState.InDeck;

        visuals.DisableCardForPlayer();
        visuals.SetUsable(true);


        base.ClearForRound();


    }

    public override void Deload()
    {
        base.Deload();

        choice = null;
    }
    #endregion


    #region Visual control

    public override void RevealCard()
    {
        base.RevealCard();
        Visible = true;
    }

    public override void HideCard()
    {
        base.HideCard();
        Visible = false;
    }

    public override IEnumerator TriggerCardEffect()
    {
        switch (state)
        {
            case HandCardState.InDeck:
            case HandCardState.InPlay:
            case HandCardState.InPile:
                yield return visuals.Pop();
                break;
            case HandCardState.InHand:
                yield return visuals.Wiggle();
                break;
            default:
                break;
        }
    }

    public override IEnumerator TriggerCardEffect(string text)
    {
        Coroutine cor = GameManager.instance.StartCoroutine(visuals.PopText(text));

        yield return TriggerCardEffect();

        yield return cor;
    }

    public IEnumerator Peek()
    {
        yield return visuals.Peek();
        Peeking = true;
    }

    public IEnumerator Return()
    {
        if (Peeking)
        {
            yield return visuals.Return();
            Peeking = false;
        }
    }
    #endregion

    #region Clicks and delegates

    public void SelectForPlay()
    {

        if (!visuals.InstantPlay)
        {
            if (Playable && owner.CanAct)
            {
                owner.selectCard(this);
            }
        }
        else if(Playable)
        {
            owner.selectedCard = this;
            owner.CanAct = false;
            owner.madeSelection = true;
        }
    }

    public void SelectForChoice()
    {
        choice.Invoke(this);
    }

    public IEnumerator PlayCard()
    {
        
        visuals.ResetSizeCard();

        owner.CanAct = false;

        visuals.SetDraggable(false);

        Coroutine move = GameManager.instance.StartCoroutine(owner.visuals.MoveCardToPlay(visuals));

        owner.PlayCard(this);
        owner.visuals.RemoveFromHand(visuals);

        visuals.OnClickedEvent = null;
        visuals.DisableCardForPlayer();

        yield return move;

        //Change later to include an animation for smoked cards playing
        Coroutine anim = GameManager.instance.StartCoroutine(owner.TriggerCard(this));
        GameManager.instance.StartCoroutine(visuals.TriggerShine(returnTempData().PostWild));

        yield return ObserverManagerSystem.NotifyPlayedCard(this);

        if (!Smoked)
        {
            yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnCardPlayed, owner, this);

            yield return owner.AddScore(ReturnCalcScore());
        }
        else
        {
            yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnSmokedPlayed, owner, this);
        }

        yield return anim;

        owner.PlaceCard(this);

        yield return new WaitForGameSeconds(0.2f);

        yield return GameManager.Round.visuals.MoveCardToPile(visuals);

        if (!Smoked)
        {
            yield return ObserverManagerSystem.NotifyPlayedCardLate(this);
        }
        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnCardPlaced, owner, this);
    }

    public IEnumerator PlayCardNoTrigger()
    {
        visuals.ResetSizeCard();

        visuals.SetDraggable(false);

        //ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, this);
        Coroutine anim = GameManager.instance.StartCoroutine(owner.TriggerCard(this));

        Coroutine move = GameManager.instance.StartCoroutine(GameManager.Round.visuals.MoveCardToPile(visuals));
        GameManager.instance.StartCoroutine(visuals.TriggerShine(returnTempData().PostWild));

        owner.PlayCard(this);
        owner.visuals.RemoveFromHand(visuals);

        visuals.OnClickedEvent = null;
        visuals.DisableCardForPlayer();

        yield return move;
        yield return anim;

        owner.PlaceCard(this);

        //yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnCardPlaced, owner, this);
    }

    public void SwitchHandToChoice(ChoiceDelegate del)
    {
        visuals.OnClickedEvent = SelectForChoice;

        choice = del;
    }
    public void SwitchChoiceToHand()
    {
        visuals.OnClickedEvent = SelectForPlay;

        choice = null;
    }

    public void DisableForChoice()
    {
        visuals.OnClickedEvent = null;
        choice = null;
    }
    #endregion

    #region Data Retrieval 
    public override HandCardData returnModifiedData()
    {
        if (owner != null)
        {
            return owner.modifyCard(tempCardData);
        }

        return tempCardData;

    }

    public HandCardData returnTempData()
    {
        return tempCardData;
    }

    public override HandCardData returnUnmodifiedData()
    {
        return CurrData;
    }

    public Colors ReturnMainColor()
    {
        if (CurrData.cardColors.Any())
        {
            return CurrData.cardColors[0];
        }
        else
            return Colors.None;
    }

    public Colors ReturnTempMainColor()
    {
        if (tempCardData.cardColors.Any())
        {
            return tempCardData.cardColors[0];
        }
        else
            return Colors.None;
    }

    public int ReturnCalcScore()
    {
        return Mathf.FloorToInt((data.CalcScore(this) + returnModifiedData().Score) * (returnModifiedData().Mult * data.CalcMult(this)));
    
    }

    public override string ReturnCardText()
    {
        if (returnModifiedData().cardValues.Count > 0)
        {
            if (returnModifiedData().cardValues[0] == 0)
            {
                return "";
            }
            return returnModifiedData().cardValues[0].ToString();
        }
        else
            return data.name[0].ToString();
    }

    public override void ShowToolTip()
    {
        float Mult = (float)Math.Round(data.CalcMult(this) * returnModifiedData().Mult, 2);

        string finalToolTip = CustomToolTip != "" ? CustomToolTip : data.ReturnDescription();

        GameManager.instance.GenerateToopTip(data.Name, finalToolTip, ReturnCalcScore().ToString(),
            (data.CalcScore(this) + returnModifiedData().Score).ToString(),
            (Mult != 1 ? Mult.ToString() : ""), TempBase, TempMult);
    }
    #endregion

    #region Temporary Changes - Add Methods
    public void SetTempValues(List<int> values, bool replace)
    {
        if (replace)
        {
            tempCardData.cardValues = new List<int>(values);
        }
        else
        {
            tempCardData.cardValues.AddRange(values);
        }
        visuals.UpdateCard(ReturnCardText());
    }

    public void SetTempColors(List<Colors> colors, bool replace)
    {
        if (replace)
        {
            tempCardData.cardColors = new List<Colors>(colors);
        }
        else
        {
            tempCardData.cardColors.AddRange(colors.Where(o => !tempCardData.cardColors.Contains(o)));
        }
        visuals.UpdateCard(tempCardData.cardColors);
    }

    public void AddTempScore(int score)
    {
        tempCardData.Score += score;

        TempBase = true;
    }

    public void SetTempScore(int score)
    {
        tempCardData.Score = score;

        TempBase = true;
    }

    public void MultTempMultScore(float mult)
    {
        tempCardData.Mult *= mult;

        TempMult = true;
    }

    public void SetTempMultScore(float mult)
    {
        tempCardData.Mult = mult;

        TempMult = true;
    }

    public void SetTempData(string NewData)
    {
        tempCardData.Name = NewData;
    }

    public void SetTempWild()
    {
        tempCardData.PostWild = true;
        tempCardData.PreWild = true;
        GameManager.instance.StartCoroutine(visuals.TriggerShine(true));
        owner.UpdateHand();
    }

    public void SetTempNotPreWild()
    {
        tempCardData.PostWild = false;
        owner.UpdateHand();
    }

    public void SetTempNotPostWild()
    {
        tempCardData.PreWild = false;
        GameManager.instance.StartCoroutine(visuals.TriggerShine(false));
    }

    public void SetTempMainColor(Colors color)
    {
        if (tempCardData.cardColors.Count > 0)
        {
            tempCardData.cardColors[0] = color;
        }
        else
        {
            tempCardData.cardColors.Add(color);
        }

        if (visuals != null)
            visuals.UpdateCard(tempCardData.cardColors);
    }

    public void SetTempMainValue(int value)
    {
        if (tempCardData.cardValues.Count > 0)
        {
            tempCardData.cardValues[0] = value;
        }
        else
        {
            tempCardData.cardValues.Add(value);
        }

        if (visuals != null)
            visuals.UpdateCard(ReturnCardText());
    }

    #endregion

    #region Permanent Changes - Set Methods
    public void SetMainValue(int value)
    {
        if (cardData.cardValues.Count > 0)
        {
            cardData.cardValues[0] = value;
            tempCardData.cardValues[0] = value;
        }
        else
        {
            cardData.cardValues.Add(value);
            tempCardData.cardValues.Add(value);
        }

        if (visuals != null)
            visuals.UpdateCard(ReturnCardText());
    }

    public void SetValues(List<int> values)
    {
        cardData.cardValues.AddRange(values);
        tempCardData.cardValues = values.Select(x => x).ToList();

        if (visuals != null)
        {
            visuals.UpdateCard(ReturnCardText());
        }
    }

    public void SetColors(List<Colors> colors)
    {
        cardData.cardColors.AddRange(colors.Where(o => !cardData.cardColors.Contains(o)));
        foreach (var item in cardData.cardColors)
        {
            tempCardData.cardColors.Add(item);
        }

        if (visuals != null)
        {
            visuals.UpdateCard(cardData.cardColors);
        }
    }

    public void SetScore(int score)
    {
        cardData.Score = score;
        tempCardData.Score = score;
    }

    public void SetMultScore(float mult)
    {
        cardData.Mult *= mult;
        tempCardData.Mult *= mult;
    }

    public void AddScore(int score)
    {
        cardData.Score += score;
        tempCardData.Score += score;
    }

    public void SetMainColor(Colors color)
    {
        if (cardData.cardColors.Count > 0)
        {
            cardData.cardColors[0] = color;
            tempCardData.cardColors[0] = color;
        }
        else
        {
            cardData.cardColors.Add(color);
            tempCardData.cardColors.Add(color);
        }

        if (visuals != null)
        visuals.UpdateCard(cardData.cardColors);
    }
    #endregion


}

