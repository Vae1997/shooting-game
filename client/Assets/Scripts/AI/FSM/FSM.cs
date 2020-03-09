using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    //Player的Transform
    protected List<Transform> playerTransform;

    //目标位置（玩家或下一个巡逻点，取决于当前状态）
    protected Vector2 destPos;

    //存放所有巡逻点
    protected GameObject[] pointList;

    //子弹速度
    protected float shootRate;
    //距上次射击的时间
    protected float elapsedTime;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

    //Use this for initialization
    void Start()
    {
        //FSM初始化
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        //每帧FSM更新
        FSMUpdate();
    }

    void FixedUpdate()
    {
        //固定时间间隔更新FSM
        FSMFixedUpdate();
    }
}
