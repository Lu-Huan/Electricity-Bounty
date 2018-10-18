using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ExitSceneCommand : Controller
{
    //退出了场景就回收所有对象
    public override void Execute(object data)
    {
        Game.Instance.ObjectPool.UnspawnAll();
    }
}