using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts.Menus.Settings
{
    public class CheckboxController : SettingsBase
    {
        bool currSetting;
        public Toggle toggle;

        public override void OnLoad(object data)
        {
            currSetting = (bool)data;
            toggle.isOn = currSetting;
        }

        public void OnTick(bool tick)
        {
            currSetting = tick;
        }

        public override object ReturnData()
        {
            return currSetting;
        }
    }
}
