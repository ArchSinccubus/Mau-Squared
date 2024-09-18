using UnityEngine;

public class WaitForGameSeconds : CustomYieldInstruction
{
    float time;

    public WaitForGameSeconds(float time)
    {
        this.time = time / GameManager.GetTimeSpeedModifier();
    }

    public override bool keepWaiting    
    {
        get 
        {
            if (!GameManager.Pause)
            {
                time -= Time.deltaTime;
            }
            return time > 0;
        }
    }
}