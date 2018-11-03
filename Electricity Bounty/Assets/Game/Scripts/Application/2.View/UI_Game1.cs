using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game1 : View {
    public override string Name
    {
        get
        {
            return Consts.V_Start1;
        }
    }

    public void Start1()
    {
        Game.Instance.LoadScene(2);
    }

    public override void HandleEvent(string eventName, object data)
    {
        
    }
}
