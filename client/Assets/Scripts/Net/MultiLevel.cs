using System.Collections.Generic;
using UnityEngine;

public class MultiLevel : MonoBehaviour {

    //每次生成的敌人数
    private int rndEnemyCount;
    //存放路点的空物体
    private Transform waypointContainer;
    //存放所有敌人的空物体
    private Transform enemyContainer;
    //玩家预制体
    public GameObject player;
    //敌人预制体
    public GameObject enemy;
    //子弹预制体
    public GameObject bullet;
    //场景最小敌人数
    private int minEnemyCount;
    //场景最大敌人数目
    private int maxEnemyCount;
    //存储固定地图路点位置信息
    public List<Vector2> wayPointsPos;
    //玩家列表
    public static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    //敌人列表
    public static Dictionary<string, GameObject> enemysDic = new Dictionary<string, GameObject>();
    //子弹列表，子弹位置同步
    public static Dictionary<string, GameObject> bullets = new Dictionary<string, GameObject>();
    //子弹标记
    public static int bulletTag = 0;
    //敌人标记
    public static int enemyTag = 0;
    //消息列表
    public static List<string> msgList = new List<string>();
    //玩家父物体
    public Transform otherPlayers;

    // Use this for initialization
    void Start()
    {
        minEnemyCount = Param.minEnemyCount;
        maxEnemyCount = Param.maxEnemyCount;
        wayPointsPos = new List<Vector2>();
        enemyContainer = GameObject.Find("enemyContainer").transform;
        waypointContainer = GameObject.Find("WaypointContainer").transform;
        //存入地图路点位置 
        for(int i = 0; i < waypointContainer.childCount; i++)
            wayPointsPos.Add(waypointContainer.GetChild(i).transform.position);
        //随机位置生成
        int rndIndex = Random.Range(0, waypointContainer.childCount);
        AddPlayer(NetAsyn.id, wayPointsPos[rndIndex]);
        //玩家添加之后，马上发送同步位置信息
        SendPos();
        //随机生成一波敌人数目,并加入列表
        CreatAndAddEnemy();
    }

    private void CreatAndAddEnemy()
    {
        rndEnemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);
        for (int i = 0; i < rndEnemyCount; i++)
        {
            CreatEnemys(enemyTag, NetAsyn.id);
            SendEnemyPos(enemyTag);
            enemyTag++;
        }
    }

    void Update()
    {
        //处理消息列表
        for (int i = 0; i < msgList.Count; i++)
            HandleMsg();
        //清除HP为0的敌人
        foreach (Transform t in enemyContainer)
        {
            if (t.GetComponent<AIController>().health == 0)
                Destroy(t.gameObject);
        }
    }

    void FixedUpdate()
    {
        //场景中没敌人，生成
        if (enemyContainer.childCount == 0)
        {
            CreatAndAddEnemy();
        }
    }

    //处理消息列表
    void HandleMsg()
    {
        //获取一条消息
        if (msgList.Count <= 0)
            return;
        string str = msgList[0];
        msgList.RemoveAt(0);
        //根据协议做不同的消息处理
        if (str == null) return;
        string[] args = str.Split(' ');
        if (args[0] == "POS")
        {
            if(args[1] == "Enemy")
            {
                OnRecvEnemyPos(args[2],args[3], args[4], args[5]);
            }
            else OnRecvPos(args[1], args[2], args[3]);
        }
        else if (args[0] == "LEAVE")
        {
            //Debug.Log("玩家:" + args[1] + "即将场景中移除");
            OnRecvLeave(args[1]);
        }
        else if(args[0] == "DIRPOS")
        {
            //Debug.Log("dirPOS:" + str);
            OnRecvDirPos(args[1], args[2], args[3]);
        }
        else if(args[0] == "BulletPOS")
        {
            //Debug.Log("BulletPOS:" + str);
            OnRecvBulletPos(args[1], args[2], args[3], args[4]);
        }
    }

    //子弹位置同步协议
    private void OnRecvBulletPos(string id, string bulletTag, string xStr, string yStr)
    {
        //不更新自己子弹的位置
        if (id == NetAsyn.id) return;
        //解析协议
        float x = float.Parse(xStr);
        float y = float.Parse(yStr);
        Vector2 pos = new Vector3(x, y);
        //已经初始化该子弹
        if (bullets.ContainsKey(id + " " + bulletTag))
        {
            if(bullets[id + " " + bulletTag] != null)
            {
                bullets[id + " " + bulletTag].transform.position = pos;
            }
        }
        //尚未初始化该子弹
        else
        {
            AddBullet(id + " " + bulletTag, pos);
        }
            
    }

    private void AddBullet(string key, Vector2 pos)
    {
        GameObject b = Instantiate(bullet, pos, Quaternion.identity);
        b.GetComponent<SpriteRenderer>().color = Color.blue;
        bullets.Add(key, b);
    }

    //处理玩家离开的协议
    public void OnRecvLeave(string id)
    {
        if (players.ContainsKey(id))
        {
            Destroy(GameObject.Find(id));
            players[id] = null;
            Debug.Log("玩家:"+id+"成功从场景中移除");
        }
    }

    //处理更新位置的协议
    public void OnRecvPos(string id, string xStr, string yStr)
    {
        //不更新自己的位置
        if (id == NetAsyn.id) return;
        //解析协议
        float x = float.Parse(xStr);
        float y = float.Parse(yStr);
        Vector2 pos = new Vector3(x, y);
        //已经初始化该玩家
        if (players.ContainsKey(id))
        {
            if(players[id] != null)
                players[id].transform.position = pos;
        }
        //尚未初始化该玩家
        else
            AddPlayer(id, pos);
    }

    private void OnRecvEnemyPos(string enemyIndex, string id, string xStr, string yStr)
    {
        //不更新自己敌人的位置
        if (id == NetAsyn.id) return;
        //解析协议
        float x = float.Parse(xStr);
        float y = float.Parse(yStr);
        Vector2 pos = new Vector3(x, y);
        //已经初始化该敌人
        if (enemysDic.ContainsKey(id + "Enemy:" + enemyIndex))
        {
            if (enemysDic[id + "Enemy:" + enemyIndex] == null) return;
            enemysDic[id + "Enemy:" + enemyIndex].transform.position = pos;
        }
            
        else
            CreatEnemys(int.Parse(enemyIndex),id);
    }

    //更新玩家瞄向的协议
    public void OnRecvDirPos(string id, string xStr, string yStr)
    {
        //不更新自己的位置
        if (id == NetAsyn.id) return;
        //解析协议
        float x = float.Parse(xStr);
        float y = float.Parse(yStr);
        Vector2 pos = new Vector3(x, y);
        //已经初始化该玩家
        if (players.ContainsKey(id))
        {
            if (players[id] != null)
                players[id].transform.GetChild(0).position = pos;
        }
            
        //尚未初始化该玩家
        else
            //AddPlayer(id, pos);
            Debug.LogError("未找到" + id + "玩家对应的dir！");
    }

    //添加玩家
    void AddPlayer(string id, Vector3 pos)
    {
        GameObject otherPlayer = Instantiate(player, pos, Quaternion.identity);
        otherPlayer.name = id;
        otherPlayer.transform.SetParent(otherPlayers);
        TextMesh textMesh = otherPlayer.GetComponentInChildren<TextMesh>();
        textMesh.text = id;
        players.Add(id, otherPlayer);
    }

    //发送位置协议
    private void SendPos()
    {
        GameObject player = players[NetAsyn.id];
        Vector2 pos = player.transform.position;
        //组装协议
        string str = "POS ";
        str += NetAsyn.id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        NetAsyn.socket.Send(bytes);
        
    }

    //发送敌人位置协议
    private void SendEnemyPos(int enemyIndex)
    {
        GameObject enemy = enemysDic[NetAsyn.id + "Enemy:" + enemyIndex];
        Vector2 pos = enemy.transform.position;
        //组装协议
        string str = "POS ";
        str += "Enemy " + enemyIndex + " " + NetAsyn.id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        NetAsyn.socket.Send(bytes);
        //Debug.Log("发送 " + str);
    }

    private void CreatEnemys(int enemyIndex, string id)
    {
        int randomIndex = Random.Range(0, waypointContainer.childCount);
        GameObject otherEnemy = Instantiate(enemy, wayPointsPos[randomIndex], Quaternion.identity);
        otherEnemy.name = id + "Enemy:" + enemyIndex;
        otherEnemy.transform.parent = enemyContainer;
        otherEnemy.AddComponent<AIController>();
        enemysDic.Add(otherEnemy.name, otherEnemy);
    }
}
