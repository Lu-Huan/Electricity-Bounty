using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

public class MapV : MonoBehaviour
{
    #region 引用
    static int MapWidth;//地图宽
    static int MapHeight;//地图高
    float TileWidth = 1;//格子宽
    float TileHeight = 1;//格子高
    List<Tile> MapTiles;
    List<Vector3> MapPath;
    int[,] dir;
    int Complexity;
    public void MapVInit(MapModel mapM)
    {
        MapWidth = MapModel.MapWidth;
        MapHeight = MapModel.MapHeight;
        MapTiles = mapM.GetMapTiles();
        MapPath = mapM.GetMapPath();
        dir = mapM.dir;
        Complexity = mapM.complexity;
    }
    bool DrawGizmos;
    //

    //地板预制件
    public GameObject End;
    private GameObject end;
    public GameObject prefab2;
    //箭头预制件
    public GameObject Arrow;

    public GameObject Door;
    private GameObject door;


    public GameObject Plane;
    public GameObject[] Mess;

    public Transform Player;
    public GameObject Effect;


    #endregion

    #region Unity回调
    /// <summary>
    /// 辅助画线
    /// </summary>
    void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;
        //格子颜色
        Gizmos.color = Color.green;

        //绘制行
        for (int row = 0; row <= MapHeight; row++)
        {
            Vector3 from = new Vector3(-TileWidth / 2, 0, -TileHeight / 2 + row * TileHeight);
            Vector3 to = new Vector3(-TileWidth / 2 + MapWidth, 0, -TileHeight / 2 + row * TileHeight);
            Gizmos.DrawLine(from, to);
        }

        //绘制列
        for (int col = 0; col <= MapWidth; col++)
        {
            Vector3 from = new Vector3(-TileWidth / 2 + col * TileWidth, 0, MapHeight - TileHeight / 2);
            Vector3 to = new Vector3(-TileWidth / 2 + col * TileWidth, 0, -TileHeight / 2);
            Gizmos.DrawLine(from, to);
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < MapPath.Count - 1; i++)
        {
            if (i == 1)
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
    /// <summary>
    /// 协程：生成地面
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetGround()
    {
        foreach (Tile item in MapTiles)
        {
            item.Data = null;
            if (!item.IsPath)
            {
                /* prefab1.transform.position = item.Position;
                 Instantiate(prefab1);*/


                int index = UnityEngine.Random.Range(0, 100);
                Thread.Sleep(20);
                if (index < Complexity / 2)
                {
                    if (Mess.Length > 0)
                    {
                        index %= Mess.Length;
                        GameObject w = Instantiate(Mess[index]);
                        w.transform.position = item.Position;
                        item.Data = w;
                    }
                }
            }
            yield return null;
        }
        CompleteMap();
    }

    /// <summary>
    /// 协程：生成箭头
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetArrow()
    {
        for (int i = 0; i < MapPath.Count - 1; i++)
        {
            Thread.Sleep(80);
            GameObject arrow = Instantiate(Arrow);
            arrow.transform.position = MapPath[i] + new Vector3(0, 0.07f, 0);

            if (i > 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 Dir = new Vector3(MapPath[i].x + dir[j, 0], 0, MapPath[i].z + dir[j, 1]);
                    if (MapPath.Contains(Dir) && Dir != MapPath[i - 1] && Dir != MapPath[i + 1])
                    {
                        GameObject go = Instantiate(Plane);
                        go.transform.position = (MapPath[i] + Dir) / 2;
                        if (MapPath[i].z == Dir.z)
                        {
                            go.transform.localEulerAngles = new Vector3(0, 90, 0);
                        }

                    }
                }
            }

            prefab2.transform.position = MapPath[i] + new Vector3(0, 0.07f, 0);
            Instantiate(prefab2);
            Vector3 vector3 = MapPath[i + 1] - MapPath[i];
            if (vector3.x == 0)
            {
                if (vector3.z == 1)
                {
                    arrow.transform.localRotation = Quaternion.Euler(new Vector3(90, -90, 0));
                }
                else
                {
                    arrow.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
                }
            }
            else
            {
                if (vector3.x == -1)
                {
                    arrow.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
                }
            }
            yield return null;
        }
        prefab2.transform.position = MapPath[MapPath.Count - 1] + new Vector3(0, 0.07f, 0);
        door = Instantiate(Door);
        door.transform.position = MapPath[0];
        end = Instantiate(End);
        end.transform.position = MapPath[MapPath.Count - 1];
        if (MapPath[1].x == MapPath[0].x)
        {
            door.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        StartCoroutine("SetDoor");
    }

    /// <summary>
    /// 协程：生成门
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetDoor()
    {
        float t0 = -1.5f;
        float t2 = 0;
        float timer = 0;
        float time = 2f;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            float f = Mathf.Lerp(t0, t2, timer / time);
            door.transform.position = new Vector3(door.transform.position.x, f, door.transform.position.z);
            end.transform.position = new Vector3(end.transform.position.x, f, end.transform.position.z);
            yield return null;
        }

    }

    /// <summary>
    /// 协程：生成玩家
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetPlayer()
    {

        Player.gameObject.SetActive(true);
        int Ro = Animator.StringToHash("Rotion");
        Effect.transform.position = Player.position;
        Effect.GetComponent<Animator>().SetBool(Ro, true);
        yield return 0;
    }

    public void Draw()
    {
        //开启3个协程

        StartCoroutine("SetPlayer");//设置主角

        StartCoroutine("SetArrow");//设置路径箭头

        StartCoroutine("SetGround");//设置地图杂物
    }

    #endregion
    #region 事件
    public event Action CompleteMap;
    #endregion
}