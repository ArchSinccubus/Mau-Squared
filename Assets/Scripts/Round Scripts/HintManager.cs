using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HintManager : MonoBehaviour
{
    public CurveContainerScriptableObject curve;

    public float speed;

    public float Height;

    float time;
    Vector2 currPos;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        currPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Vector2.Lerp(currPos, currPos + Vector2.up * Height, curve.returnValue(time));
        transform.localPosition = new Vector3(0, move.y, transform.position.z);
        time += Time.deltaTime * speed;
        //transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);

    }

}

