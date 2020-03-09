using System.Collections.Generic;
using UnityEngine;

//AI控制脚本
public class AIController : AdvancedFSM
{
    //行走路径
    public AIPath path = new AIPath();

    //敌人子弹预制体
    public Transform bulletPrefab;

    //AI的HP
    public int health = 1;

    public Vector2 curPoint;

    //AI的FSM初始化
    protected override void Initialize()
    {
        playerTransform = new List<Transform>();

        //上次射击时间
        elapsedTime = 0.0f;

        //射击频率
        shootRate = 2f;
        
        //获取所有玩家transform
        GameObject []objPlayer = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < objPlayer.Length; i++)
            playerTransform.Add(objPlayer[i].transform);
        if (playerTransform.Count == 0)
        {
            Debug.LogError("玩家不存在，初始化失败");
            return;
        }
        //构建FSM
        ConstructFSM();
    }

    //每帧更新
    protected override void FSMUpdate()
    {
        curPoint = path.wayPoint;
        //距上次射击时间
        elapsedTime += Time.deltaTime;
        //死亡
        if (health <= 0)
        {
            SetTransition(Transition.NoHealth);
            //死亡变色
            transform.GetComponent<SpriteRenderer>().color = Color.red;
            //死亡层级为0
            transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            //死亡可通过,移除无用组件
            Destroy(transform.GetComponent<CircleCollider2D>());
            Destroy(transform.GetComponent<Rigidbody2D>());
            Destroy(transform.GetComponent<AIController>());
            //死亡Tag重置
            transform.tag = "Untagged";
            //从敌人删去
            if(ReadyBtn.isMultiGame)
                Destroy(transform.gameObject);
            else
                Level.enemys.Remove(transform.gameObject);
        }
    }

    protected override void FSMFixedUpdate()
    {
        //最近玩家
        Transform player = GetCloselyPlayer(playerTransform);
        //玩家死亡
        if (player.GetComponent<Player>().Hp == 0)
            player = null;
        //当前状态进行转换
        CurrentState.Reason(player, transform);
        //在新的状态下ACT
        CurrentState.Act(player, transform);
        //更新路点
        if (path.IsReach(transform))
            path.NextWaypoint();
    }

    //获取离当前敌人最近的玩家
    private Transform GetCloselyPlayer(List<Transform> playerTransform)
    {
        float minDistance = float.MaxValue;
        Transform closelyPlayer = null;
        for (int i = 0; i < playerTransform.Count; i++)
        {
            if(playerTransform[i] == null)
            {
                playerTransform.RemoveAt(i);
                continue;
            }
            float dis = Vector2.Distance(playerTransform[i].position , transform.position);
            if (dis < minDistance)
            {
                minDistance = dis;
                closelyPlayer = playerTransform[i];
            }
        }
        if (!closelyPlayer)
            Debug.LogError("获取最近玩家失败");
        return closelyPlayer;
    }

    //设置转换
    public void SetTransition(Transition t)
    {
        //根据转换设置新状态
        PerformTransition(t);
    }

    //FSM构建方法
    private void ConstructFSM()
    {
        //存放所有路点
        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");
        //所有路点的transform
        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach (GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }
        //描述AI的状态转换图
        PatrolState patrol = new PatrolState(waypoints);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        ChaseState chase = new ChaseState(waypoints);
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        AttackState attack = new AttackState(waypoints);
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        DeadState dead = new DeadState();
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(dead);
    }

    //可视化调试
    void OnDrawGizmos()
    {
       path.DrawWaypoints();
    }

    //AI射击
    public void ShootBullet(Vector2 aiShootDir,Vector2 bulletPos)
    {
        if(elapsedTime >= shootRate)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletPos, transform.rotation).gameObject;
            //修改颜色区别子弹
            bullet.GetComponent<SpriteRenderer>().color = Color.red;
            //子弹发射
            bullet.GetComponent<Rigidbody2D>().velocity = aiShootDir * bullet.GetComponent<Bullet>().Speed * Time.deltaTime;
            elapsedTime = 0f;
        }
    }
}
