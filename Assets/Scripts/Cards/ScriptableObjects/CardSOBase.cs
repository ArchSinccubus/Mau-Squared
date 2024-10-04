using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CardSOBase : ScriptableObject, ISubscriber
{
    public string Name;
    public Sprite CardFront;

    public List<AIStrategy> AIStrategyType;
    public bool AIChoice, AIAffectsSelf;

    public int ScoreAmount;
    public float MultAmount;
    public int ChoiceAmount;
    public int MoneyAmount;
    public int NumberAmount;

    public virtual bool ChoiceCard => false;

    public virtual bool Tarot => false;

    public virtual int BasePrice
    {
        get {
            switch(Rarity) 
            {
                case CardRarity.Common: return 3;
                case CardRarity.Uncommon: return 4;
                case CardRarity.Rare: return 6;
                case CardRarity.Legendary: return 9;
            }
            return 3;
        }
    }
    public CardRarity Rarity;

    public virtual bool RoundEvents => true;

    public virtual bool SilentTrigger => false;

    public virtual bool SideCard { get { return false; } }

    public virtual void Subscribe(object subscriber)
    {
        
    }

    public virtual void Unsubscribe(object subscriber)
    { 
        
    }

    public virtual void OnPickup(BaseCardDataHandler card)
    {
        
    }

    public virtual void OnRemove(BaseCardDataHandler card)
    { 
        
    }

    public virtual bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return false;
    }

    public virtual string ReturnDescription()
    {
        return "";
    }

    public virtual string ReturnChoiceText()
    {
        return "";   
    }

    public virtual string ReturnTriggeredText()
    {
        return "";
    }

    public string ConvertStringToValues(string s)
    {
        return s.Replace("+{}", ScoreAmount.ToString()).
            Replace("x{}", MultAmount.ToString()).
            Replace("c{}", ChoiceAmount.ToString()).
            Replace("m{}", MoneyAmount.ToString()).
            Replace("n{}", NumberAmount.ToString());
    }
}
