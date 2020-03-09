using UnityEngine;
using System.Collections.Generic;

//转换编号
public enum Transition
{
    SawPlayer = 0, //发现玩家
    ReachPlayer,    //接近玩家
    LostPlayer,     //失去玩家
    NoHealth,       //死亡
}

//状态编号
public enum FSMStateID
{
    Patrolling = 0, //巡逻
    Chasing,        //追击
    Attacking,      //进攻
    Dead,           //死亡
}

public class AdvancedFSM : FSM
{
    //FSM状态列表
    private List<FSMState> fsmStates;

    //当前状态及编号
    private FSMStateID currentStateID;
    public FSMStateID CurrentStateID { get { return currentStateID; } }

    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }

    public AdvancedFSM()
    {
        //新建列表
        fsmStates = new List<FSMState>();
    }

    //添加状态
    public void AddFSMState(FSMState fsmState)
    {
        //空状态
        if (fsmState == null) Debug.LogError("FSM ERROR: Null reference is not allowed");
        //加入第一个状态
        if (fsmStates.Count == 0)
        {
            fsmStates.Add(fsmState);
            currentState = fsmState;
            currentStateID = fsmState.ID;
            return;
        }
        //添加的状态已存在
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == fsmState.ID)
            {
                Debug.LogError("FSM ERROR: Trying to add a state that was already inside the list");
                return;
            }
        }
        //添加
        fsmStates.Add(fsmState);
    }

    //删除状态 
    public void DeleteState(FSMStateID fsmState)
    {
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == fsmState)
            {
                fsmStates.Remove(state);
                return;
            }
        }
        //删除的状态不存在
        Debug.LogError("FSM ERROR: The state passed was not on the list. Impossible to delete it");
    }

    //当前状态转换为新状态
    public void PerformTransition(Transition trans)
    {
        //设置新的状态编号
        currentStateID = currentState.GetOutputState(trans);
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == currentStateID)
            {
                //设置新的状态
                currentState = state;
                break;
            }
        }
    }
}
