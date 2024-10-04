using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BackroundManager : MonoBehaviour
{
    public Transform InLoc, OutLoc;

    public Image Background;

    public Image Floor;

    public IEnumerator MoveBackroundOut()
    {
        Vector2 target = OutLoc.transform.position;
        Vector2 currPos = Floor.transform.position;

        yield return MoveBackround(currPos, target);
    }

    public IEnumerator MoveBackroundIn(Sprite image)
    {
        Background.transform.position = InLoc.position;
        Background.sprite = image;

        Vector2 target = Floor.transform.position;
        Vector2 currPos = InLoc.transform.position;

        yield return MoveBackround(currPos, target);
    }

    public IEnumerator MoveBackround(Vector2 currPos, Vector2 target)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["quicktoss"];

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            Vector2 move = Vector2.Lerp(currPos, target, curve.returnValue(time));
            Background.transform.position = new Vector3(move.x, move.y, transform.position.z);
            time += Time.deltaTime / 2;
            yield return new WaitForEndOfFrame();
        }
        Background.transform.position = new Vector3(target.x, target.y, transform.position.z);
    }

    public void PutBackgroundOut()
    {
        Background.transform.position = OutLoc.transform.position;
    }

    public void PutBackgroundIn(Sprite image)
    {
        Background.sprite = image;


        Background.transform.position = Floor.transform.position;
    }
}
