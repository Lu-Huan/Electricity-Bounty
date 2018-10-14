using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Threading;

public enum Dir
{
    Up,
    Down,
    Left,
    Right
}
public class Map : MonoBehaviour
{
    #region 常量
    public const int RowCount = 8;  //行数
    public const int ColumnCount = 12; //列数
    public const int PathLength = 20; //长度
    #endregion

    #region 字段
    float MapWidth = 12;//地图宽
    float MapHeight = 8;//地图高

    float TileWidth = 1;//格子宽
    float TileHeight = 1;//格子高

    List<Tile> MapTiles = new List<Tile>();//格子集合
    List<Vector3> MapPath = new List<Vector3>();//路径集合

    //辅助划线
    public bool DrawGizmos;
    //
    public int RoadLength=15;
    //地板预制件
    public GameObject prefab;

    //用于深搜的数据
    private int CurrentLength=0;
    private int[,] Path = new int[12, 8];
    int[,] dir = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
    Stack<Vector2> Road = new Stack<Vector2>();
    #endregion

    #region 方法
    public void LoadLevel()
    {
        //清除当前状态
        Clear();
    }
    //清除所有信息
    public void Clear()
    {
        foreach (Tile t in MapTiles)
        {
            if (t.IsPath)
                t.IsPath = false;
        }

        MapPath.Clear();
    }

    #endregion

    #region Unity回调
    void Awake()
    {
        //创建所有的格子
        CreateTile();


        //得到随机路径
        CreateRandomPath();

        //设置路径
        SetTilePath();


        foreach (Tile item in MapTiles)
        {
            if (!item.IsPath)
            {
                Instantiate(prefab);
                prefab.transform.position = item.Position;
            }
        }
        InvokeRepeating("CreateRandomPath", 1f, 2f); 

    }
    void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;
        //格子颜色
        Gizmos.color = Color.green;

        //绘制行
        for (int row = 0; row <= RowCount; row++)
        {
            Vector3 from = new Vector3(-TileWidth / 2, 0, -TileHeight / 2 + row * TileHeight);
            Vector3 to = new Vector3(-TileWidth / 2 + MapWidth, 0, -TileHeight / 2 + row * TileHeight);
            Gizmos.DrawLine(from, to);
        }

        //绘制列
        for (int col = 0; col <= ColumnCount; col++)
        {
            Vector3 from = new Vector3(-TileWidth / 2 + col * TileWidth, 0, MapHeight - TileHeight / 2);
            Vector3 to = new Vector3(-TileWidth / 2 + col * TileWidth, 0, -TileHeight / 2);
            Gizmos.DrawLine(from, to);
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < MapPath.Count - 1; i++)
        {
            if(i==1)
            {
                Gizmos.color = Color.red;
            }
            Vector3 from = MapPath[i];
            Vector3 to = MapPath[i + 1];
            Gizmos.DrawLine(from, to);
        }
    }
    #endregion

    #region 帮助方法
    public void CreateRandomPath()
    {

        MapInit();

        int x = UnityEngine.Random.Range(1, ColumnCount - 1);
        Thread.Sleep(200);

        int y = UnityEngine.Random.Range(1, RowCount - 1);
        //= CreateRandomPoint(true);
        /*Vector2 End = CreateRandomPoint(false, (int)Start.x);

        Path[(int)End.x, (int)End.y] = 2;*/
        DFS_Start(x, y);

         Debug.LogFormat("开始:({0},{1})",x,y);
         //Debug.Log(End);
        Array array = Road.ToArray();
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
        MapPath.Add(Pa[Pa.Count - 1]);
        SetTilePath();
    }
    //设置路径
    private void SetTilePath()
    {
        foreach (Vector3 item in MapPath)
        {
            int x = (int)item.x;
            int y = (int)item.z;
            Tile tile = GetTile(x, y);
            tile.IsPath = true;
        }
    }
    //生成随机点（添加限制）
    private Vector2 CreateRandomPoint(bool frist, int x1 = 0)
    {
        Vector2 Point = new Vector2();
        int t = UnityEngine.Random.Range(0, 4);
        Thread.Sleep(200);
        int x;
        if (frist)
        {
            x = UnityEngine.Random.Range(0, ColumnCount);
        }
        else if (x1 < ColumnCount / 2)
        {
            x = UnityEngine.Random.Range(ColumnCount / 2, ColumnCount);
        }
        else
        {
            x = UnityEngine.Random.Range(0, ColumnCount / 2);
        }
        Thread.Sleep(200);
        int y = UnityEngine.Random.Range(0, RowCount);
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
                Point = new Vector2(ColumnCount - 1, y);
                break;
            case 3:
                Point = new Vector2(x, RowCount - 1);
                break;
            default:
                throw new IndexOutOfRangeException("随机数异常");
        }
        return Point;
    }



    /// <summary>
    /// 初始化
    /// </summary>
    private void MapInit()
    {
        CurrentLength = 0;
        Road.Clear();
        for (int i = 0; i < 12; i++)
            for (int j = 0; j < 8; j++)
                Path[i, j] = 0;
        Clear();
    }
    //设置搜索模式（+1或+2）
    private int RandomMode()
    {
        int mode = UnityEngine.Random.Range(1, 3);
        return mode;
    }
    /// <summary>
    /// 判断是否可走
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>

    private bool Check(int X, int Y)
    {
        if (X > 0 && X < ColumnCount-1 && Y >= 0 && Y < RowCount && Path[X, Y] != 1)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 输入一个点，返回周围可走的区域
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int[] SerchAround(int x, int y,int mode)
    {
        int[] a = new int[5];
        int t = 0;
        for (int i = 0; i < 4; i++)
        {
            if (Check(x + dir[i, 0], y + dir[i, 1]))
            {
                if (mode == 2)
                {
                    if (Check(x + dir[i, 0] *mode, y + dir[i, 1] * mode))
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
        Debug.LogFormat("查:({0},{1}),有{2}个可走", x, y, t);
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
        //Debug.Log("入" + new Vector2(x2, y2));
        Road.Push(new Vector2(X, Y));

        CurrentLength += Mode;

        if (CurrentLength>=RoadLength)
        {
            // Debug.Log("找到了");
            return true;
        }
        int[] next = SerchAround(X, Y,Mode);

        for (int i = 0; i < next[4]; i++)
        {
            if (i != 0)
                next = SerchAround(X, Y, Mode);
            if (next[4] == 0)
            {
                return false;
            }
            int index = UnityEngine.Random.Range(0, next[4]);
            Thread.Sleep(10);
            //Debug.Log("index="+index);
            // Debug.Log("next[index]=" + next[index]);
            //Debug.LogFormat("({0},{1})", x2 + dir[next[index], 0], y2 + dir[next[index], 1]);
            int nextX = dir[next[index], 0]*Mode;
            int nextY = dir[next[index], 1]*Mode;
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
        Road.Pop();
        CurrentLength -= Mode;
        return false;
    }
    //获得格子的方法
    //根据位置获得索引
    //根据格子索引号获得格子
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
        int index = tileX + tileY * ColumnCount;
        if (index < 0 || index >= MapTiles.Count)
            throw new IndexOutOfRangeException("格子索引越界");
        return MapTiles[index];
    }
    //创建抽象格子
    private void CreateTile()
    {
        for (int i = 0; i < RowCount; i++)
            for (int j = 0; j < ColumnCount; j++)
            {
                Vector3 TilePos = new Vector3(j, 0, i);
                MapTiles.Add(new Tile(TilePos));
            }
    }

    /*//获取鼠标下面的格子
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