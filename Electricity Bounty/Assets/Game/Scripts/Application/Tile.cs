using UnityEngine;
using UnityEditor;

public class Tile
{
    public Vector3 Position;

    public object Data; //格子所保存的数据

    public bool IsPath;

    public Tile(Vector3 pos)
    {
        Position = pos;
        IsPath = false;
        Data = null;
    }
   

}