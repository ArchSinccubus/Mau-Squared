using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Auxilary
{

    public class WaitForGameEndOfFrame : CustomYieldInstruction
    {
        float time;

        public override bool keepWaiting
        {
            get
            {
                return GameManager.Pause;
            }
        }
    }


}
