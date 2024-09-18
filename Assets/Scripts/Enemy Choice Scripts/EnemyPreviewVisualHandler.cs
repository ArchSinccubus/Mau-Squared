using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPreviewVisualHandler : MonoBehaviour
{
    public TextMeshProUGUI NameText;

    public TextMeshProUGUI AITypeText;

    public TextMeshProUGUI AIStratText;

    public TextMeshProUGUI AIColorText;

    public List<ICardVisuals> Sides;

    public CardContainer SideCardPreviewContainer;

    public Button SelectButton;

    public int num;

    public void InitVisuals(NPCHandler data, int num)
    {
        this.num = num;

        Sides = new List<ICardVisuals>();

        NameText.text = "Name";

        AITypeText.text = TypeToText(data.AI.AIType);

        AIStratText.text = StrategyToText(data.AI.preferredStrategy);

        AIColorText.text = ColorToText(data.AI.preferredColor);

        SideCardPreviewContainer.Init(GridLayoutGroup.Constraint.FixedRowCount, 1, true);

        foreach (var item in data.SideCards)
        {
            item.InitForView(true, 0, false);
            Sides.Add(item.visuals);
            item.visuals.RevealCard();
        }

        SideCardPreviewContainer.SetupSlots(GameManager.getCardVisuals(data.SideCards.ToArray()));

        foreach (var item in data.SideCards)
        {
            SideCardPreviewContainer.PutNewCard(item.visuals);
        }

    }

    public void Deload()
    {
        try
        {
            SideCardPreviewContainer.DestroyCards();
        }
        catch { }
    }

    public string TypeToText(AIType type)
    {
        switch (type)
        {
            case AIType.Random:
                break;
            case AIType.Dumb:
                break;
            case AIType.Average:
                break;
            case AIType.Smart:
                break;
            case AIType.Master:
                break;
        }

        return type.ToString();
    }

    public string StrategyToText(AIStrategy strat)
    {
        switch (strat)
        {
            case AIStrategy.None:
                break;
            case AIStrategy.HandEmpty:
                break;
            case AIStrategy.HighScore:
                break;
            case AIStrategy.SmokeOpponent:
                break;
            case AIStrategy.SmokeSelf:
                break;
            case AIStrategy.GetMoney:
                break;
            case AIStrategy.PileControl:
                break;
            case AIStrategy.Random:
                break;
        }

        return strat.ToString();
    }

    public string ColorToText(Colors color)
    {
        switch (color)
        {
            case Colors.Red:
                break;
            case Colors.Orange:
                break;
            case Colors.Blue:
                break;
            case Colors.Green:
                break;
            case Colors.None:
                break;
        }

        return color.ToString();
    }
}
