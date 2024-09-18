using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SideCardDataHandler : BaseCardDataHandler
{
    [JsonProperty]
    public string DataName;

    [JsonIgnore]
    public SideCardSO data;

    [JsonProperty]
    private bool playerControl;

    public bool Used;

    public override bool PlayerControl
    {
        get { return playerControl; }
        set
        {
            playerControl = value;

            if (value)
            {
                visuals.EnableCardForPlayer();
            }
            if (!value)
            {
                visuals.DisableCardForPlayer();
            }
        }

    }
    public SideCardDataHandler(SideCardSO newData, EntityHandler owner, bool temp) : base(newData, owner, temp)
    { 
        data = newData;
        DataName = newData.name;
    }

    public SideCardDataHandler(SideCardSO newData) : base(newData)
    {
        data = newData;
        DataName = newData.name;

        TempBool = false;

        Used = true;
    }

    public SideCardDataHandler(SideCardDataHandler copy) : base(copy)
    {
        data = copy.data;
        DataName = copy.DataName;

        Used = copy.Used;

        TempBool = copy.TempBool;
        TempData1 = copy.TempData1;
        TempData2 = copy.TempData2;
        PermData1 = copy.PermData1;

        this.owner = copy.owner;
    }

    [JsonConstructor]
    public SideCardDataHandler(string DataName, bool playerControl, bool used, long iD, EntityHandler owner, bool temp, object tempData1, object tempData2, object permData1, bool tempBool) : 
        base(iD, owner, temp, tempData1, tempData2, permData1, tempBool)
    {
        this.DataName = DataName;
        data = GameManager.instance.AssetLibrary.FetchSideCard(DataName.Replace("'", ""));
        baseData = data;
        Used = used;
    }

    public override void InitForRound(bool highlight)
    {
        if (visuals == null)
        {
            InitVisuals(true, data, highlight && data.Clickable, 0, false);
        }

        visuals.RevealCard();

        if (data.Clickable)
        {
            Used = false;
            visuals.OnClickedEvent = DoCommand;
            PlayerControl = true;
            visuals.Pulsing = true;
        }


        if (data is ISubscriber)
        {
            (data as ISubscriber).Subscribe(this);
        }
    }

    public override void InitForView(bool side, int Canvasoverride, bool Col)
    {
        if (visuals == null)
        {
            InitVisuals(true, data, false, Canvasoverride, Col);
        }
    }

    public override void InitForChoice(bool highlight, ChoiceDelegate del, int Canvasoverride)
    {
        if (visuals == null)
        {
            InitVisuals(true, data, highlight, Canvasoverride, false);
        }

        visuals.RevealCard();

        visuals.EnableCardForPlayer();

        visuals.OnClickedEvent = SelectForChoice;

        choice += del;
    }

    public void DoCommand()
    {
        if (owner.CanAct && !Used && data.Clickable)
        {
            GameManager.Round.StartCoroutine(CommandCoroutine());
            owner.ActivatedEffects = true;
            Used = true;
            owner.CanAct = false;
        }
    }

    public override IEnumerator TriggerCardEffect()
    {
        if (GameManager.currRun.runState == GameState.InRound)
        {
            if (GameManager.Round.state != RoundState.EndRound)
            {
                owner.ActivatedEffects = true;
            }
        }
        yield return visuals.Pop();
    }

    public override IEnumerator TriggerCardEffect(string text)
    {
        Coroutine cor = GameManager.instance.StartCoroutine(visuals.PopText(text));

        yield return TriggerCardEffect();
        yield return cor;
    }

    public IEnumerator CommandCoroutine()
    {
        visuals.Pulsing = false;
        yield return TriggerCardEffect();

        yield return new WaitForGameSeconds(0.2f);

        yield return data.DoCommand(this);

        yield return new WaitForGameSeconds(0.2f);

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnActivateSideCard, owner, this);

        owner.CanAct = true;
    }

    public void RefreshCard()
    {
        Used = false;
        visuals.Pulsing = true;
    }

    public void SelectForChoice()
    {
        choice.Invoke(this);
    }

    public override void ShowToolTip()
    {
        GameManager.instance.GenerateToopTip(data.Name, data.ReturnDescription(), "", "", "", false, false);
    }

    public override void InitVisuals(bool side, CardSOBase data, bool highlight, int CanvasOverride, bool Col)
    {
        ICardVisuals vis = PoolingManager.DrawFromPool(side, Col);
        (vis as SideCardVisualHandler).initSide(data, highlight, data.SideCard, ReorganizeCards, CanvasOverride);
        visuals = vis;
        visuals.OnToolTip = ShowToolTip;
    }

    public override void ClearForRound()
    {
        if (baseData != data)
        {
            data = GameManager.instance.AssetLibrary.FetchSideCard(baseData.Name);
        }

        base.ClearForRound();
    }

    public void ReorganizeCards()
    {
        owner.ChangeSideCardOrder();
    }
}
