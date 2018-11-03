using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Level
{
    //名字
    public string Name;

    //卡片
    public string CardImage;

    //背景
    public string Background;

    //路径
    public string Road;

    //金币
    public int InitScore;

    //怪物行走的路径
    public List<Vector3> Path = new List<Vector3>();

    //出怪回合信息
    public List<Round> Rounds = new List<Round>();
}