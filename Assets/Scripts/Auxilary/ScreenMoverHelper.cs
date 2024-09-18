using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMoverHelper : MonoBehaviour
{
    public Transform HideLoc, ShowLoc;

    public IEnumerator MoveScreen(bool show)
    {
        Vector2 target = show ? ShowLoc.position : HideLoc.position; 

        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["toss"];

        Vector2 currPos = transform.position;

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            Vector2 move = Vector2.Lerp(currPos, target, curve.returnValue(time * GameManager.GetTimeSpeed()));
            transform.position = new Vector3 (move.x, move.y, transform.position.z);
            time += Time.deltaTime;

            yield return new WaitForGameEndOfFrame();
        }

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }

    public void PutScreen(bool show)
    {
        Vector2 target = show ? ShowLoc.position : HideLoc.position;

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }
}
