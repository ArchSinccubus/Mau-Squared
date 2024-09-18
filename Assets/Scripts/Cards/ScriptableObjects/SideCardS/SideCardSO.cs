using Assets.Scripts.Auxilary;
using System;
using System.Collections;


public class SideCardSO : CardSOBase
{
    public virtual bool Clickable => false;

    public override bool SideCard { get { return true; } }

    public virtual bool Copyable => true;

    public virtual bool Triggerable => false;

    public virtual void InitCard(SideCardDataHandler card)
    { 
        
    }

    public virtual void initForCollection(SideCardDataHandler card)
    {
        
    }

    public virtual IEnumerator DoCommand(SideCardDataHandler card)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public override string ReturnDescription()
    {
        string raw = GameManager.instance.AssetLibrary.GetSideDescription(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }

    public override string ReturnChoiceText()
    {
        string raw = GameManager.instance.AssetLibrary.GetSideChoiceText(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }

    public override string ReturnTriggeredText()
    {
        string raw = GameManager.instance.AssetLibrary.GetSideTrigger(Name.Replace(",", ""));

        return ConvertStringToValues(raw);
    }
}
