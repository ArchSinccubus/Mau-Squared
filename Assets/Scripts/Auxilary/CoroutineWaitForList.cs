using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineWaitForList : CustomYieldInstruction, IDisposable
{
    List<IEnumerator> listOfCouritnes;

    public CoroutineWaitForList() 
    { 
        listOfCouritnes = new List<IEnumerator>();
    }

    public IEnumerator CountCoroutine(IEnumerator coroutine)
    {
        listOfCouritnes.Add(coroutine);
        yield return coroutine;
        listOfCouritnes.Remove(coroutine);
    }

    public void Dispose()
    {
        listOfCouritnes.Clear();
        listOfCouritnes = null;
    }

    public override bool keepWaiting
    {
        get
        {
            return listOfCouritnes.Count != 0 || GameManager.Pause;
        }
    }
}
