using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public IEnumerator Trigger(HandCardDataHandler card);

    public void Subscribe(HandCardDataHandler card);
}
