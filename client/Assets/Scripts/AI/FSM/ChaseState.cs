using UnityEngine;

//追击状态
public class ChaseState : FSMState
{
    public ChaseState(Transform[] wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Chasing;
        curRotSpeed = 6.0f;
        curSpeed = 100.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {
        if (player == null) return;
        float dist = Vector3.Distance(npc.position, player.position);
        //在进攻范围内
        if (dist <= attackDistance)
            //进攻
            npc.GetComponent<AIController>().SetTransition(Transition.ReachPlayer);
        //在追击范围外
        else if (dist >= chaseDistance)
            //巡逻
            npc.GetComponent<AIController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(Transform player, Transform npc)
    {
        if (player == null) return;
        //获取脚本
        AIController aiCtrl = npc.GetComponent<AIController>();
        //目标位置
        destPos = player.position;
        if (aiCtrl.path.pathArray == null || aiCtrl.path.isFinish)
            aiCtrl.path.InitByAStarPath(npc.position, destPos);
        //npc位置
        Vector2 npcPos = new Vector2(npc.position.x, npc.position.y);
        
        //方向
        Vector2 dir = aiCtrl.path.wayPoint - npcPos;
        //转向
        float targetRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 30;
        //在T时间内完成旋转
        npc.rotation = Quaternion.Slerp(npc.rotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * curRotSpeed);
        //移动
        npc.GetComponent<Rigidbody2D>().velocity = dir.normalized * curSpeed * Time.deltaTime;
        if(ReadyBtn.isMultiGame)
            SendPos(npc);
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
