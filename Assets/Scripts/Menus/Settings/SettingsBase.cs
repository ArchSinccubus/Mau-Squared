using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsBase : MonoBehaviour
{
    public string Name;

    public virtual void OnLoad(object data)
    { 
        
    }

    public virtual object ReturnData()
    {
        return null;
    }
}
