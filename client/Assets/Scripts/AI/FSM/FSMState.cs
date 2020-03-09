using UnityEngine;
using System.Collections.Generic;

public abstract class FSMState
{
    //转换-状态字典
    protected Dictionary<Transition, FSMStateID> map = new Dictionary<Transition, FSMStateID>();
    //状态编号
    protected FSMStateID stateID;
    public FSMStateID ID { get { return stateID; } }
    //目标位置
    protected Vector2 destPos;
    //巡逻点
    protected Transform[] waypoints;
    //转动速度
    protected float curRotSpeed;
    //移动速度
    protected float curSpeed;
    //追击范围
    protected float chaseDistance = 50.0f;
    //进攻范围
    protected float attackDistance = 10.0f;
    //路点半径
    protected float arriveDistance = 1.0f;

    //添加转换
    public void AddTransition(Transition transition, FSMStateID id)
    {
        //添加的转换已存在
        if (map.ContainsKey(transition))
        {
            Debug.LogWarning("FSMState ERROR: transition is already inside the map");
            return;
        }
        //添加
        map.Add(transition, id);
    }

    //删除转换
    public void DeleteTransition(Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        //删除的转化不存在
        Debug.LogError("FSMState ERROR: Transition passed was not on this State磗 List");
    }

    //根据转换得到新的状态
    public FSMStateID GetOutputState(Transition trans)
    {
        return map[trans];
    }

    //确定是否转换
    public abstract void Reason(Transform player, Transform npc);

    //当前状态执行的操作
    public abstract void Act(Transform player, Transform npc);

    //随机得到巡逻点
    public void FindNextPoint()
    {
        int rndIndex = Random.Range(0, waypoints.Length);
        int rndX = Random.Range(0, 3);
        int rndY = Random.Range(0, 3);
        //随机干扰量
        Vector3 rndPosition = new Vector2(rndX, rndY);
        destPos = waypoints[rndIndex].position + rndPosition;
    }
}
