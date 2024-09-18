using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
    public Button ResumeB, MenuB, SettingsB;
    public TextMeshProUGUI text;

    public MenuMoverHelper helper;

    public IEnumerator OpenPauseMenu()
    {
        gameObject.SetActive(true);
        if (GameManager.currRun != null)
        {
            if (GameManager.currRun.RunSeed != "")
            {
                text.text = GameManager.currRun.RunSeed;
            }
        }

        yield return helper.MoveScreen(true);
    }

    public void CloseMenu()
    {
        StartCoroutine(CloseMenuCoru());
    }

    public IEnumerator CloseMenuCoru()
    {
        yield return helper.MoveScreen(true);
        gameObject.SetActive(false);
    }
}
