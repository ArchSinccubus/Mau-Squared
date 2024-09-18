using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;


public class CardMenuHandler : MonoBehaviour
{
    public Canvas canvas;

    public CardContainer cardContainer;

    public List<BaseCardDataHandler> options;

    protected HandCardDataHandler[] choice;

    public TextMeshProUGUI choiceText;
    public Button CloseButton;
    public Button ChooseButton;

    public int RowCount;

    bool finish, force;

    CardMenuChoiceMode choiceMode;

    /// <summary>
    /// Inits a menu for any reason
    /// </summary>
    /// <param name="cards">Cards to put in the menu</param>
    /// <param name="text">Any accompanying text to put in the menu</param>
    /// <param name="AllowClose">Should the menu be closable via button?</param>
    /// <param name="ChoiceMode">What kind of choice mode should the menu habe?</param>
    /// <param name="view">The type of constraints on the menu group</param>
    /// <param name="row">How many rows should the menu have at most?</param>
    public void initMenu(BaseCardDataHandler[] cards, string text, bool AllowClose, CardMenuChoiceMode ChoiceMode, MenuViewMode view, int row)
    {
        cardContainer.Init(GridLayoutGroup.Constraint.FixedRowCount, row, false);

        options = new List<BaseCardDataHandler>();

        foreach (var item in cards)
        {
            switch (view)
            {
                case MenuViewMode.View:
                    item.InitForView(false, canvas.sortingOrder, false);
                    break;
                case MenuViewMode.Choice:
                    item.InitForChoice(true, PickCard, canvas.sortingOrder);
                    break;
                default:
                    break;
            }
            options.Add(item);
        }

        choiceText.text = text;
        choiceMode = ChoiceMode;
        CloseButton.gameObject.SetActive(AllowClose);
        ChooseButton.gameObject.SetActive(ChoiceMode != CardMenuChoiceMode.Null);

    }

    public void initHandChoice(string text, bool AllowClose, CardMenuChoiceMode ChoiceMode)
    {
        choiceText.text = text;
        choiceMode = ChoiceMode;

        CloseButton.gameObject.SetActive(AllowClose);
    }

    //TODO: Gonna have to change that later cause I'm gonna need a way to tell the menu text on the cards it shows
    public IEnumerator MakeChoice(int MaxChoice)
    {
        gameObject.SetActive(true);

        CoroutineWaitForList list = new CoroutineWaitForList();

        cardContainer.SetupSlots(GameManager.getCardVisuals(options.ToArray()));

        foreach (var item in options)
        {
            cardContainer.PutNewCard(item.visuals);
            item.PlayerControl = true;
        }
        yield return list;

        choice = new HandCardDataHandler[MaxChoice];
        finish = false;


        while (!finish)
        {
            yield return new WaitForGameEndOfFrame();
        }

        HandCardDataHandler[] temp = new HandCardDataHandler[MaxChoice];

        for (int i = 0; i < choice.Length; i++)
        {
            if (choice[i] != null)
            {
                temp[i] = new HandCardDataHandler(choice[i]);
            }
        }

        yield return temp;
    }

    public IEnumerator MakeHandChoice(CardContainer hand,List<HandCardDataHandler> handData, int MaxChoice, Predicate<HandCardDataHandler> query)
    {
        Transform origParent = hand.transform.parent;

        gameObject.SetActive(true);

        hand.transform.SetParent(transform);

        foreach (var item in handData)
        {
            if (query != null)
            {
                if (!query(item))
                {
                    item.DisableForChoice();
                }
                else
                {
                    item.SwitchHandToChoice(PickCard);
                    item.visuals.SetCardToMenu(canvas.sortingOrder);
                }
            }
            else
            {
                item.SwitchHandToChoice(PickCard);
                item.visuals.SetCardToMenu(canvas.sortingOrder);
            }
        }

        finish = false;
        choice = new HandCardDataHandler[MaxChoice];

        while (!finish)
        {
            yield return new WaitForGameEndOfFrame();
        }

        choice = choice.Where(o => o != null).ToArray();

        for (int i = 0; i < choice.Length; i++)
        {
            choice[i].visuals.SetSelect(false);
        }

        hand.transform.SetParent(origParent);

        foreach (var item in handData)
        {
            item.SwitchChoiceToHand();
            item.visuals.SetCardToHand();
        }

        yield return choice;
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);

        cardContainer.SetupSlots(GameManager.getCardVisuals(options.ToArray()));

        foreach (var item in options)
        {
            cardContainer.PutNewCard(item.visuals);
            item.visuals.RevealCard();
            item.visuals.SetDraggable(true);
            item.visuals.UpdateCard(item.returnModifiedData().cardColors, item.ReturnCardText());
        }
    }

    public virtual void PickCard(BaseCardDataHandler card)
    {
        if (!choice.Contains(card))
        {
            if (choice.Length == 1)
            {
                if (choice[0] != null)
                {
                    choice[0].visuals.SetSelect(false);
                }
                choice[0] = card as HandCardDataHandler;
                choice[0].visuals.SetSelect(true);
            }
            else
            {
                for (int i = 0; i < choice.Length; i++)
                {
                    if (choice[i] == null)
                    {
                        choice[i] = card as HandCardDataHandler;
                        card.visuals.SetSelect(true);
                        break;
                    }
                }
            }
        }
        else
        {
            card.visuals.SetSelect(false);
            for (int i = 0; i < choice.Length; i++)
            {
                if (choice[i] == card)
                {
                    choice[i] = null;
                    break;
                }
            }
        }
    }

    public void FinishChoice()
    {
        switch (choiceMode)
        {
            case CardMenuChoiceMode.Forced:
                finish = choice.Where(o => o == null).Count() == 0;
                break;
            case CardMenuChoiceMode.Semi:
                finish = choice.Where(o => o == null).Count() != choice.Length;
                break;
            case CardMenuChoiceMode.Open:
                finish = true;
                break;
            default:
                break;
        }
    }

    public virtual void CloseMenu()
    {
        cardContainer.DestroyCards();

        options.Clear();

        if (choice != null)
        {
            Array.Clear(choice, 0, choice.Length);
        }

        gameObject.SetActive(false);
        finish = true;
    }

    public void CloseHandMenu()
    {
        gameObject.SetActive(false);
        finish = true;
    }
}
