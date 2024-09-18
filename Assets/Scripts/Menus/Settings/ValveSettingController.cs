using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ValveSettingController : SettingsBase, IPointerUpHandler
{
    public UnityEvent PointerUpEvent;

    float value;
    public Slider slider;

    public override void OnLoad(object data)
    {
        value = (float)data;
        slider.value = value;
    }

    public void OnChange(float newValue)
    { 
        value = newValue;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpEvent.Invoke();
    }

    public override object ReturnData()
    {
        return value;
    }
}
