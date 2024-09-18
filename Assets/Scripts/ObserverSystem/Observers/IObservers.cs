using System.Collections;


public interface IObserverOnCardPlayed : ISubscriber
{
    public IEnumerator TriggerOnCardPlayed(EventDataArgs args);
}

public interface IObserverOnCardPlaced : ISubscriber
{
    public IEnumerator TriggerOnCardPlaced(EventDataArgs args);
}

public interface IObserverOnSmokedPlayed : ISubscriber
{
    public IEnumerator TriggerOnSmokedPlayed(EventDataArgs args);
}

public interface IObserverOnEndRound : ISubscriber
{
    public IEnumerator TriggerOnEndRound(EventDataArgs args);
}

public interface IObserverOnEndTurn : ISubscriber
{
    public IEnumerator TriggerOnEndTurn(EventDataArgs args);
}

public interface IObserverOnPicked : ISubscriber
{
    public IEnumerator TriggerOnPicked(EventDataArgs args);
}

public interface IObserverOnCardSmoked : ISubscriber
{
    public IEnumerator TriggerOnSmoked(EventDataArgs args);
}

//Card is unsmoked
public interface IObserverOnCardCleaned : ISubscriber
{
    public IEnumerator TriggerOnCleaned(EventDataArgs args);
}

public interface IObserverOnCardRemoved : ISubscriber
{
    public IEnumerator TriggerOnRemoved(EventDataArgs args);
}

public interface IObserverOnStartRound : ISubscriber
{
    public IEnumerator TriggerStartRound(EventDataArgs args);
}

public interface IObserverOnPreStartRound : ISubscriber
{
    public IEnumerator TriggerPreStartRound(EventDataArgs args);
}

public interface IObserverOnStartTurn : ISubscriber
{
    public IEnumerator TriggerOnStartTurn(EventDataArgs args);
}

public interface IObserverOnRecycle : ISubscriber
{
    public IEnumerator TriggerOnRecycle(EventDataArgs args);
}

public interface IObserverOnDraw : ISubscriber
{
    public IEnumerator TriggerOnDraw(EventDataArgs args);
}

public interface IObserverOnBuy : ISubscriber
{
    public IEnumerator TriggerOnBuy(EventDataArgs args);
}

public interface IObserverOnSideSell : ISubscriber
{
    public IEnumerator TriggerOnSideSell(EventDataArgs args);
}

public interface IObserverOnRemove : ISubscriber
{
    public IEnumerator TriggerOnRemove(EventDataArgs args);
}

public interface IObserverOnRefresh : ISubscriber
{
    public IEnumerator TriggerOnRefresh(EventDataArgs args);
}

public interface IObserverOnShopExit : ISubscriber
{
    public IEnumerator TriggerOnShopExit(EventDataArgs args);
}

public interface IObserverOnShopEnter : ISubscriber
{
    public IEnumerator TriggerOnShopEnter(EventDataArgs args);
}

public interface IObserverOnScoreChanged: ISubscriber
{    
    public IEnumerator TriggerOnScoreChanged(EventDataArgs args);
}

public interface IObserverOnMoneyChanged : ISubscriber
{
    public IEnumerator TriggerOnMoneyChanged(EventDataArgs args);
}

public interface IObserverOnActivateSideCard : ISubscriber
{
    public IEnumerator TriggerOnActivateSideCard(EventDataArgs args);

}
