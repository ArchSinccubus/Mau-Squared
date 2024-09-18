using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubscriber
{
    public void Subscribe(object subscriber);

    public void Unsubscribe(object subscriber);
}