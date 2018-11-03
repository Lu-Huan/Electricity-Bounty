using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Round
{

    public int Monster;
  
    public int Count;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="monster">怪物类型(ID索引)</param>
    /// <param name="count">怪物数量</param>
    public Round(int monster, int count)
    {
        this.Monster = monster;
        this.Count = count;
    }
}