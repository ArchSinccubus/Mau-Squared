using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class BlackoutHandler : MonoBehaviour
{
    public float speed;

    public Image blackout;

    public IEnumerator FadeScreen(bool show)
    {
        float init = blackout.color.a;
        Color c= blackout.color;

        float target = show ? 1 : 0;
        if (target == 1)
        {
            blackout.raycastTarget = true;
        }

        float time = 0;

        while (blackout.color.a != target) 
        {
            c.a = Mathf.Lerp(init, target, time);
            time += Time.deltaTime * speed;

            blackout.color = c;

            yield return new WaitForEndOfFrame();
        }

        if (target == 0)
        {
            blackout.raycastTarget = false;
        }

        c.a = target;
        blackout.color = c;
    }
}
