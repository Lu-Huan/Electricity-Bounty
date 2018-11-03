using UnityEngine;
using UnityEditor;

public class UI_ShowLevelImformation : View
{
    public override string Name
    {
        get
        {
            return Consts.V_ShowLevelImformation;
        }
    }
    public override void RegisterEvents()
    {
        AttentionEvents.Add(Consts.E_StartLevel);
    }
    public override void HandleEvent(string eventName, object data)
    {
        
    }
}