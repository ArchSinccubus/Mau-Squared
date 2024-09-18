using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Auxilary
{
    [Serializable]
    [CreateAssetMenu(fileName = "InputValidator - NumChar.asset", menuName = "TextMeshPro/Input Validators/NumChar", order = 100)]
    public class TextNumInputValidator : TMP_InputValidator
    {
        public int TextLimit;

        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (text.Length < TextLimit)
            {
                if (ch >= 33 && ch <= 126)
                {
                    text += ch;
                    pos += 1;
                    return ch;
                }
            }

            

            return (char)0;
        }
    }
}
