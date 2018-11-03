using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(ObjectPool))]
[RequireComponent(typeof(Sound))]
[RequireComponent(typeof(StaticData))]
public class Game : ApplicationBase<Game>
{
    //全局访问功能
    [HideInInspector] public ObjectPool ObjectPool = null; //对象池
    [HideInInspector] public Sound Sound = null;//声音控制
    [HideInInspector] public StaticData StaticData = null;//静态数据

    //全局方法
    public void LoadScene(int level)
    {
        //---退出旧场景
        //造一个事件参数
        SceneArgs e = new SceneArgs()
        {
            SceneIndex = SceneManager.GetActiveScene().buildIndex //当前场景索引
        };
        //发布事件
        SendEvent(Consts.E_ExitScene, e);

        //---加载新场景
        SceneManager.LoadScene(level, LoadSceneMode.Single);
        SceneManager.sceneLoaded += LoadedEve;
    }

    void LoadedEve(Scene s, LoadSceneMode l)
    {
        if (l == LoadSceneMode.Single)
        {
            SceneManager.sceneLoaded -= LoadedEve;
            //事件参数
            SceneArgs e = new SceneArgs() { SceneIndex = s.buildIndex };

            //发布事件
            SendEvent(Consts.E_EnterScene, e);
        }
    }

    //游戏入口
    void Start()
    {
        //确保Game对象一直存在
        DontDestroyOnLoad(gameObject);

        //全局单例赋值
        ObjectPool = ObjectPool.Instance;
        Sound = Sound.Instance;
        StaticData = StaticData.Instance;

        //注册启动命令
        RegisterController(Consts.E_StartUp, typeof(StartUpCommand));

        //启动游戏
        SendEvent(Consts.E_StartUp);
    }
}
