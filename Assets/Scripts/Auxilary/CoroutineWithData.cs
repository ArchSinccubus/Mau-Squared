using System;
using System.Collections;
using UnityEngine;

public class CoroutineWithData<T> 
{
    public Coroutine coroutine { get; private set; }


    public T result;
    private IEnumerator target;

    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext() && !GameManager.Pause)
        {
            if (target.Current is T)
            {
                result = (T)target.Current;
                //Debug.Log("Returned data! " + target.ToString());
            }
            yield return result;
        }
    }
}