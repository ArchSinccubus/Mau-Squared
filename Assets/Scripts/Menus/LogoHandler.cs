using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoHandler : MonoBehaviour
{
    public GameObject Elements;

    public float MaxAngle, MinAngle;

    // Update is called once per frame
    void Update()
    {
        Elements.transform.localRotation = Quaternion.Euler(0, 0, getAngle());
    }

    public float getAngle()
    {
       return Mathf.Lerp(MaxAngle, MinAngle, (Mathf.Sin(Time.time) + 1) /2);
    }
}
