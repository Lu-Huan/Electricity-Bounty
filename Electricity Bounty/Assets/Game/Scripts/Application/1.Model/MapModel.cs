using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Threading;

public class MapModel:Model
{
    public MapModel()
    {
        CreateTile();
        LoadMap(30,50);
    }
    #region 常量
    public static int MapWidth = 14;//地图宽
    public static int MapHeight = 10;//地图高
    #endregion

    #region 字段
    float TileWidth = 1;//格子宽
    float TileHeight = 1;//格子高

    private List<Tile> MapTiles = new List<Tile>();//格子集合
    private List<Vector3> MapPath = new List<Vector3>();//路径集合
    private int Complexity;

    public int complexity
    {
        set
        {
            Complexity = Mathf.Clamp(value, 0, 100);
        }
        get
        {
            return Complexity;
        }
    }

    //用于深搜的数据
    private int CurrentLength = 0;//当前长度

    private int[,] Path = new int[MapWidth, MapHeight];//路径表

    public int[,] dir = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };//方向

    private Stack<Vector2> Road = new Stack<Vector2>();//用栈来存路径

    public int RoadLength = 35;

    
    #endregion

    #region 对外接口
    /// <summary>
    /// 生成一条路径
    /// </summary>
    /// <param name="legth">路径长</param>
    /// <param name="complexity">复杂度</param>
    public void LoadMap(int legth,int complexity)
    {
        RoadLength = legth;
        Complexity = complexity;

        //清除当前状态
        Clear();

        //得到随机路径
        CreateRandomPath();

        //设置怪物的路径
        SetTilePath();
    }

    public List<Vector3> GetMapPath()
    {
        return MapPath;
    }

    public List<Tile> GetMapTiles()
    {
        return MapTiles;
    }

    public override string Name
    {
        get
        {
            return Consts.M_MapModel;
        }
    }
    #endregion

    #region 内部封装
    /// <summary>
    /// 产生随机路径
    /// 算法入口
    /// </summary>
    private void CreateRandomPath()
    {

        MapInit();

        int x = UnityEngine.Random.Range(1, MapWidth );
        Thread.Sleep(200);

        int y = UnityEngine.Random.Range(1, MapHeight);
        //= CreateRandomPoint(true);
        /*Vector2 End = CreateRandomPoint(false, (int)Start.x);

        Path[(int)End.x, (int)End.y] = 2;*/
        DFS_Start(x, y);
        //Debug.LogFormat("开始:({0},{1})",x,y);
        //Debug.Log(End);
        Array array = Road.ToArray();
        Road.Clear();
        List<Vector3> Pa = new List<Vector3>();
        foreach (Vector2 item in array)
        {
            Pa.Add(new Vector3(item.x, 0, item.y));
        }
        for (int i = 0; i < Pa.Count - 1; i++)
        {
            //Debug.Log("Pa"+Pa[i]);
            Vector3 v1 = Pa[i];
            Vector3 v2 = Pa[i + 1];
            MapPath.Add(v1);
            if (Mathf.Abs((v1 - v2).x + (v1 - v2).z) == 2)
            {
                MapPath.Add((v2 + v1) / 2);
            }
        }
        if (Pa.Count > 1)
        {
            MapPath.Add(Pa[Pa.Count - 1]);
        }

        SetTilePath();
    }

    /// <summary>
    /// 设置怪的路径
    /// </summary>
    private void SetTilePath()
    {
        foreach (Vector3 item in MapPath)
        {
            Tile tile = GetTile((int)item.x, (int)item.z);
            tile.IsPath = true;
        }
    }

    /// <summary>
    /// 生成随机点（添加限制）
    /// </summary>
    /// <param name="frist"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    private Vector2 CreateRandomPoint(bool frist, int x1 = 0)
    {
        Vector2 Point = new Vector2();
        int t = UnityEngine.Random.Range(0, 4);
        Thread.Sleep(200);
        int x;
        if (frist)
        {
            x = UnityEngine.Random.Range(0, MapWidth);
        }
        else if (x1 < MapWidth / 3)
        {
            x = UnityEngine.Random.Range(MapWidth / 2, MapWidth);
        }
        else
        {
            x = UnityEngine.Random.Range(0, MapWidth / 2);
        }
        Thread.Sleep(200);
        int y = UnityEngine.Random.Range(0, MapHeight);
        Thread.Sleep(200);
        switch (t)
        {
            case 0:
                Point = new Vector2(0, y);
                break;
            case 1:
                Point = new Vector2(x, 0);
                break;
            case 2:
                Point = new Vector2(MapWidth - 1, y);
                break;
            case 3:
                Point = new Vector2(x, MapHeight - 1);
                break;
            default:
                throw new IndexOutOfRangeException("随机数异常");
        }
        return Point;
    }

    /// <summary>
    /// 清除所有信息
    /// </summary>
    private void Clear()
    {
        foreach (Tile t in MapTiles)
        {
            if (t.IsPath)
                t.IsPath = false;
        }

        MapPath.Clear();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void MapInit()
    {
        CurrentLength = 0;
        Road.Clear();
        for (int i = 0; i < MapWidth; i++)
            for (int j = 0; j < MapHeight; j++)
                Path[i, j] = 0;
        Clear();
    }

    /// <summary>
    /// 设置搜索模式（+1或+2）
    /// </summary>
    /// <returns></returns>
    private int RandomMode()
    {
        int mode = UnityEngine.Random.Range(0, 100);
        if (mode < Complexity)
        {
            mode = 1;
        }
        else
        {
            mode = 2;
        }
        return mode;
    }

    /// <summary>
    /// 判断是否可走
    /// </summary>
    /// <param name="X">坐标x</param>
    /// <param name="Y">坐标y</param>
    /// <returns>返回bool值</returns>
    private bool Check(int X, int Y)
    {
        if (X > 0 && X < MapWidth - 1 && Y > 0 && Y < MapHeight - 1 && Path[X, Y] != 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 输入一个点(x,y)，返回周围可走的区域
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int[] SerchAround(int x, int y, int mode)
    {
        int[] a = new int[5];
        int t = 0;
        for (int i = 0; i < 4; i++)
        {
            if (Check(x + dir[i, 0], y + dir[i, 1]))
            {
                if (mode == 2)
                {
                    if (Check(x + dir[i, 0] * mode, y + dir[i, 1] * mode))
                    {
                        a[t] = i;
                        t++;
                    }
                    else
                        continue;
                }
                else
                {
                    a[t] = i;
                    t++;
                }

            }
        }
        a[4] = t;
        // Debug.LogFormat("查:({0},{1}),有{2}个可走", x, y, t);
        /*for (int i = 0; i < t; i++)
        {
            switch (a[i])
            {
                case 0:
                    Debug.Log("上");
                    break;
                case 1:
                    Debug.Log("下");
                    break;
                case 2:
                    Debug.Log("右");
                    break;
                case 3:
                    Debug.Log("左");
                    break;
            }

        }*/
        return a;
    }

    /// <summary>
    /// 深度搜索
    /// 新的点（X,Y）
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    private bool DFS_Start(int X, int Y)
    {
        int Mode = RandomMode();
        //Debug.LogFormat("({0},{1})", x, y);
        Path[X, Y] = 1;

        /*if (Mathf.Abs(x2-x1+y2-y1) == 2)
        {
            //Debug.Log("中间："+new Vector2((x2 + x1) / 2, (y2 + y1) / 2));
            Path[(x2 + x1) / 2, (y2 + y1) / 2] = 1;
        }*/
        //Debug.Log("入" + new Vector2(X, Y));
        Road.Push(new Vector2(X, Y));

        CurrentLength += Mode;

        if (CurrentLength >= RoadLength)
        {
            // Debug.Log("找到了");
            return true;
        }
        int[] next = SerchAround(X, Y, Mode);

        for (int i = 0; i < next[4]; i++)
        {
            if (i != 0)
                next = SerchAround(X, Y, Mode);
            if (next[4] == 0)
            {
                break;
            }
            int index = UnityEngine.Random.Range(0, next[4]);
            Thread.Sleep(10);
            //Debug.Log("index=" + index);
            // Debug.Log("next[index]=" + next[index]);
            //Debug.LogFormat("({0},{1})", x2 + dir[next[index], 0], y2 + dir[next[index], 1]);
            int nextX = dir[next[index], 0] * Mode;
            int nextY = dir[next[index], 1] * Mode;
            if (Mode == 2)
            {
                Path[X + nextX / 2, Y + nextY / 2] = 1;
            }
            if (DFS_Start(X + nextX, Y + nextY))
            {
                return true;
            }
        }
        //Debug.Log("出" + new Vector2(x2, y2));
        Path[X, Y] = 0;
        Road.Pop();
        CurrentLength -= Mode;
        return false;
    }

    //获得格子的方法
    //根据位置获得索引 
    public Tile GetTile(Vector3 position)
    {
        //对主角坐标进行转换
        int tileX = Mathf.RoundToInt(position.x);
        int tileY = Mathf.RoundToInt(position.y);

        return GetTile(tileX, tileY);
    }

    public Tile GetTile(int tileX, int tileY)
    {
        //计算索引
        //根据格子索引号获得格子
        int index = tileX * MapHeight + tileY;
        if (index < 0 || index >= MapTiles.Count)
            throw new IndexOutOfRangeException("格子索引越界");
        return MapTiles[index];
    }

    /// <summary>
    /// 创建抽象格子
    /// </summary>
    private void CreateTile()
    {
        for (int i = 0; i < MapWidth; i++)
            for (int j = 0; j < MapHeight; j++)
            {
                Vector3 TilePos = new Vector3(i, 0, j);
                MapTiles.Add(new Tile(TilePos));
            }
    }


    /*//暂时不用的方法
    Tile GetTileUnderMouse()
    {
        Vector2 wordPos = GetWorldPosition();
        return GetTile(wordPos);
    }

    //获取鼠标所在位置的世界坐标
    Vector3 GetWorldPosition()
    {
        Vector3 viewPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewPos);
        return worldPos;
    }*/
    #endregion
}