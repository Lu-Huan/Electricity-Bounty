using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapV))]
public class Spawner : View
{
    MapV MapV;
    Electricity electricity = null;



    public override string Name
    {
        get
        {
            return Consts.V_Spanwner;
        }
    }

    public override void RegisterEvents()
    {
        AttentionEvents.Add(Consts.E_EnterScene);
        AttentionEvents.Add(Consts.E_SpawnMonster);
        AttentionEvents.Add(Consts.E_SpawnTower);
    }
    private void SpawnMonster(int MonsterID)
    {
        string prefabName = "Monster" + MonsterID;
        GameObject go = Game.Instance.ObjectPool.Spawn(prefabName);



        Monster monster = go.GetComponent<Monster>();
        monster.Reached += monster_Reached;
        monster.HpChanged += monster_HpChanged;
        monster.Dead += monster_Dead;

        MapModel mapM = GetModel<MapModel>();
        monster.Load(mapM.GetMapPath());
    }

    private void monster_HpChanged(int arg1, int arg2)
    {
        throw new NotImplementedException();
    }

    void monster_Reached(Monster monster)
    {
        //掉血
        //electricity.Damage(1);
        Debug.Log(monster.name + "怪物到了终点");

        //怪物死亡
        monster.Hp = 0;
    }

    void monster_Dead(Role monster)
    {
        //怪物回收
        Game.Instance.ObjectPool.Unspawn(monster.gameObject);

        //胜利条件判断
        RoundModel rm = GetModel<RoundModel>();
        GameModel gm = GetModel<GameModel>();
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        if (monsters.Length == 0        //场景里没有怪物了
            && !electricity.IsDead          //还活着
            && rm.AllRoundsComplete)    //所有怪物都已出完
        {
            //游戏胜利
            SendEvent(Consts.E_EndLevel, new EndLevelArgs() { LevelID = gm.PlayLevelIndex, IsSuccess = true });
        }
    }
    public override void HandleEvent(string eventName, object data)
    {
        switch (eventName)
        {
            case Consts.E_EnterScene:
                SceneArgs sce = data as SceneArgs;
                if (sce.SceneIndex == 3)
                {
                    MapV = GetComponent<MapV>();
                    MapModel MapM = GetModel<MapModel>();
                    MapV.MapVInit(MapM);
                    MapV.Draw();
                    MapV.CompleteMap += Complete;
                }

                break;
            case Consts.E_SpawnMonster:
                SpawnMonsterArgs s = data as SpawnMonsterArgs;
                Debug.Log("生成怪物:" + s.MonsterID);
                SpawnMonster(s.MonsterID);
                break;
            case Consts.E_SpawnTower:
                Debug.Log("生成塔");
                break;
        }
    }
    void Complete()
    {
        SendEvent(Consts.E_StartLevel, null);
    }
}
