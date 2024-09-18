using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuHandler : MonoBehaviour
{
    public MenuMoverHelper helper;

    public IEnumerator OpenCreditsMenu()
    {
        gameObject.SetActive(true);

        yield return helper.MoveScreen(true);
    }

    public void CloseMenu()
    {
        StartCoroutine(CloseMenuCoru());
    }

    public IEnumerator CloseMenuCoru()
    {
        yield return helper.MoveScreen(false);
        gameObject.SetActive(false);
    }
}
