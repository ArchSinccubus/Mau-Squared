using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICardDecorator
{
    public HandCardData Decorate(object caller, HandCardData card);
}

public interface IPlayerDecorator 
{ 
    public PlayerData Decorate(PlayerData player);
}
