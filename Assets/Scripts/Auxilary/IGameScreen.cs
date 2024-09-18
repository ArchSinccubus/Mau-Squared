using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public interface IGameScreen
{
    public Transform transform { get; }

    public ScreenMoverHelper MoverHelper { get; }

    public void Deload();

    public void SetScreenActive(bool active);

    public IEnumerator MoveScreen(bool show);

    public void PutScreen(bool show);
}

