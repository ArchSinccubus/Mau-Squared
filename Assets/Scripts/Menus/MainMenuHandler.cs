using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class MainMenuHandler : MonoBehaviour
{
    public Button Start, Settings, Exit;

    public StartMenuHandler StartMenu;
    public SettingsMenuHandler SettingsMenu;

    private void Awake()
    {
        
    }

    public void ShowStart()
    {
        StartCoroutine(ShowStartScreen());
    }

    public void ShowSettings()
    {
        GameManager.instance.ShowSettings(false);
    }

    public IEnumerator ShowStartScreen()
    { 
        StartMenu.gameObject.SetActive(true);
        yield return StartMenu.helper.MoveScreen(true);
    }

    public void HideStart()
    {
        StartCoroutine(HideScreen());
    }

    public IEnumerator HideScreen()
    {
        yield return StartMenu.helper.MoveScreen(false);
        StartMenu.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
