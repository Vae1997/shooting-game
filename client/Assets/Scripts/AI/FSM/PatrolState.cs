using UnityEngine;

//巡逻状态
public class PatrolState : FSMState
{
    public PatrolState(Transform[] wp)
    {       
        //传入巡逻点
        waypoints = wp;
        //设置状态号
        stateID = FSMStateID.Patrolling;
        //设置速度
        curRotSpeed = 6.0f;
        curSpeed = 80.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {
        if (player == null) return;
        //距离小于追击距离
        if (Vector3.Distance(npc.position, player.position) <= chaseDistance)
            //追击
            npc.GetComponent<AIController>().SetTransition(Transition.SawPlayer);
    }

    public override void Act(Transform player, Transform npc)
    {
        //获取脚本
        AIController aiCtrl = npc.GetComponent<AIController>();
        //路径为空或上个路径已经完成，继续随机寻找路径
        if (aiCtrl.path.pathArray == null || aiCtrl.path.isFinish)
        {
            FindNextPoint();
            aiCtrl.path.InitByAStarPath(npc.position, destPos);
        }
        //npc位置
        Vector2 npcPos = new Vector2(npc.position.x, npc.position.y);
        //确定方向
        Vector2 dir = aiCtrl.path.wayPoint - npcPos;
        //移动
        npc.GetComponent<Rigidbody2D>().velocity = dir.normalized * curSpeed * Time.deltaTime;
        if (ReadyBtn.isMultiGame)
            SendPos(npc);
        //旋转角度转为弧度
        float targetRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg +30;
        //在T时间内完成旋转
        npc.rotation = Quaternion.Slerp(npc.rotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * curRotSpeed);
    }

    //发送位置协议
    private void SendPos(Transform npc)
    {
        Vector2 pos = npc.position;
        //组装协议
        string str = "POS ";
        str += "Enemy " + npc.name.Split(':')[1] + " " + NetAsyn.id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        NetAsyn.socket.Send(bytes);
        //Debug.Log("发送 " + str);
    }
}
