using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    //生成的玩家实体
    private GameObject playerIns;
    //生成的敌人实体
    private GameObject enemyIns;
    //敌人是否挂载脚本
    private bool enemyIsOk;
    //敌人生成总次数
    private int rndBattleCount;
    //已经生成敌人次数
    private int haveBattleCount;
    //每次生成的敌人数
    private int rndEnemyCount;
    //房间的最大最小宽度，地图的最大长宽，房间的个数
    private int room_max_length,
        room_max_width,
        room_min_length,
        room_min_width,
        map_max_length,
        map_max_width, 
        room_num,
        min_corridor_len,
        max_corridor_len,
        step;
    //存放地牢的空物体
    private Transform dungeon;
    //存放路点的空物体
    private Transform waypointContainer;
    //存放所有敌人的空物体
    private Transform enemyContainer;
    //玩家预制体
    private GameObject player;
    //敌人预制体
    private GameObject enemy;
    //最小敌人波次
    private int minBattleCount;
    //最大敌人波次
    private int maxBattleCount;
    //场景最小敌人数
    private int minEnemyCount;
    //场景最大敌人数目
    private int maxEnemyCount;
    //地图生成成功后，才可以开始网格管理器
    public static bool mapIsOK;
    //战斗开始标记
    public static bool startBattle;
    //场景中所有生存的敌人
    public static List<GameObject> enemys;

    // Use this for initialization
    void Start ()
    {
        room_max_length = Param.room_max_length;
        room_max_width = Param.room_max_width;
        room_min_length = Param.room_min_length;
        room_min_width = Param.room_min_width;
        map_max_length = Param.map_max_length;
        map_max_width = Param.map_max_width;
        room_num = Param.room_num;
        min_corridor_len = Param.min_corridor_len;
        max_corridor_len = Param.max_corridor_len;
        step = Param.step;
        minBattleCount = Param.minBattleCount;
        maxBattleCount = Param.maxBattleCount;
        minEnemyCount = Param.minEnemyCount;
        maxEnemyCount = Param.maxEnemyCount;
        player = Resources.Load("Player") as GameObject;
        enemy = Resources.Load("Enemy") as GameObject;
        enemys = new List<GameObject>();
        enemyContainer = GameObject.Find("enemyContainer").transform;
        dungeon = GameObject.Find("dungeon").transform;
        waypointContainer = GameObject.Find("WaypointContainer").transform;
        haveBattleCount = 0;
        //随机战斗次数
        rndBattleCount = UnityEngine.Random.Range(minBattleCount , maxBattleCount + 1);
        //未挂脚本
        enemyIsOk = false;
        //生成地图
        CreatCurLevelMap();
        //随机位置生成玩家
        CreatPlayer();
        //随机生成第一波敌人数目
        rndEnemyCount = UnityEngine.Random.Range(minEnemyCount, maxEnemyCount + 1);
        for (int i = 0; i < rndEnemyCount; i++)
            CreatEnemys(i);
        //将第一波敌人加入列表
        GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < e.Length; i++)
            enemys.Add(e[i]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //最后一个tip点击之后，战斗开始
        if (startBattle)
        {
            //一轮战斗结束
            if (enemys.Count == 0)
            {
                enemyIsOk = false;
                haveBattleCount++;
                //通关
                if (haveBattleCount == rndBattleCount)
                {
                    //路点列表清空
                    CreateDragon.wayPointsPos.Clear();
                    //下一关提示出现
                    GameObject.Find("Canvas").transform.GetChild(9).gameObject.SetActive(true);
                    //清除地牢
                    foreach (Transform t in dungeon)
                        Destroy(t.gameObject);
                    //清楚敌人
                    foreach (Transform t in enemyContainer)
                        Destroy(t.gameObject);
                    //清除路点
                    foreach (Transform t in waypointContainer)
                        Destroy(t.gameObject);
                    //删除网格控制器上的Level脚本
                    Destroy(GameObject.Find("GridManager").transform.GetComponent<Level>());
                    //删除网格控制器上的GridManager脚本
                    Destroy(GameObject.Find("GridManager").transform.GetComponent<GridManager>());
                }
                else
                {
                    //生成下一波敌人
                    rndEnemyCount = UnityEngine.Random.Range(minEnemyCount, maxEnemyCount + 1);
                    for (int i = 0; i < rndEnemyCount; i++)
                        CreatEnemys(i);
                    //将新一波敌人加入列表
                    GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");
                    for (int i = 0; i < e.Length; i++)
                        enemys.Add(e[i]);
                }
            }
            //给敌人挂脚本
            else if (!enemyIsOk)
            {
                for (int i = 0; i < enemys.Count; i++)
                    enemys[i].AddComponent<AIController>();
                enemyIsOk = true;
            }
        }
    }

    private void CreatCurLevelMap()
    {
        Map.Instance.Init(room_max_length, room_max_width, room_min_length, room_min_width, map_max_length, map_max_width, room_num, min_corridor_len, max_corridor_len);//初始化参数
        Map.Instance.Make_dungeon(step);//生成地牢
        Action<Transform, Vector2> instantiate = (t, v) =>
        {
            Transform g = Instantiate(t, v, t.rotation);
            if(g.name.Contains("wall"))
                g.SetParent(dungeon);
            if (g.name.Contains("waypoint"))
                g.SetParent(waypointContainer);
        };
        CreateDragon.Instance.Creat(Map.Instance.Getmap(), instantiate);
        mapIsOK = true;
    }

    private void CreatPlayer()
    {
        int randomIndex = UnityEngine.Random.Range(0, CreateDragon.wayPointsPos.Count);
        playerIns = Instantiate(player, CreateDragon.wayPointsPos[randomIndex],Quaternion.identity);
        playerIns.name = "Player";
    }

    private void CreatEnemys(int enemyIndex)
    {
        int randomIndex = UnityEngine.Random.Range(0, CreateDragon.wayPointsPos.Count);
        enemyIns = Instantiate(enemy, CreateDragon.wayPointsPos[randomIndex], Quaternion.identity);
        enemyIns.name = "Enemy" + enemyIndex;
        enemyIns.transform.parent = enemyContainer;
    }
}
