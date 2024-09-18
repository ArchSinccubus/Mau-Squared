using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Menus.Settings
{
    public class DropdownSettingController : SettingsBase
    {
        public int currSetting;
        public TMPro.TMP_Dropdown dropdown;

        public override void OnLoad(object data)
        {
            currSetting = (int)data;
            dropdown.value = currSetting;
        }

        public void OnChange(int newSetting)
        { 
            currSetting = newSetting;
        }

        public override object ReturnData()
        {
            return currSetting;
        }
    }
}
