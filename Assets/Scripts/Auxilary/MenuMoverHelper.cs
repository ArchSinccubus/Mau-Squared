using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MenuMoverHelper : MonoBehaviour
{
    public Transform HideLoc, ShowLoc;

    public IEnumerator MoveScreen(bool show)
    {
        Vector2 target = show ? ShowLoc.position : HideLoc.position;

        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["quicktoss"];

        Vector2 currPos = transform.position;

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            Vector2 move = Vector2.Lerp(currPos, target, curve.returnValue(time));
            transform.position = new Vector3(move.x, move.y, transform.position.z);
            time += Time.deltaTime * 2;

            yield return null;
        }

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }

    public void PutScreen(bool show)
    {
        Vector2 target = show ? ShowLoc.position : HideLoc.position;

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }
}

