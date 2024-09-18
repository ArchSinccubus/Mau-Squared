using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpeedSetController : SettingsBase
{
    public UnityEvent OnChange;

    public TextMeshProUGUI CurrText;

    public GameSpeed currSetting;

    public override void OnLoad(object data)
    {
        currSetting = (GameSpeed)data;
        CurrText.text = currSetting.ToString();
    }

    public void ChangeRight()
    {
        if ((int)currSetting + 1 >= Enum.GetValues(typeof(GameSpeed)).Length)
        {
            currSetting = 0;
        }
        else
        {
            currSetting++;
        }

        CurrText.text = currSetting.ToString();

        OnChange.Invoke();
    }

    public void ChangeLeft()
    {
        if ((int)currSetting - 1 < 0)
        {
            currSetting = (GameSpeed)(Enum.GetValues(typeof(GameSpeed)).Length - 1);
        }
        else
        {
            currSetting--;
        }

        CurrText.text = currSetting.ToString();

        OnChange.Invoke();
    }

    public override object ReturnData()
    {
        return currSetting;
    }
}
